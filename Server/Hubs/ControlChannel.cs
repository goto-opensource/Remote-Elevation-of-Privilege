using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using rEoP.Shared.Model;

namespace rEoP.Server.Hubs
{
    public class ControlChannel : Hub
    {
        private readonly ISessionStore _store;

        public ControlChannel(ISessionStore store) : base()
        {
            _store = store;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var sessionsWithUser = _store.Sessions.Where(s => s.Value.OriginalOwner.UserId == this.Context.UserIdentifier || s.Value.Players.Any(p => p.UserId == this.Context.UserIdentifier)).Select(s => s.Value);
            foreach (var s in sessionsWithUser)
            {
                s.Disconnected.TryAdd(this.Context.UserIdentifier, new object());
                if (s.Disconnected.Count == s.Players.Count + (s.OwnerPlaying ? 0 : 1))
                {
                    Task.Run(() =>
                    {
                        Debug.WriteLine($"cleanup for session {s.IDHash} started");
                        // background worker service - "schedule" and iterate on timestampned sessions, hangfire
                        Thread.Sleep((int)TimeSpan.FromHours(6).TotalMilliseconds);
                        _store.Sessions.TryRemove(s.IDHash, out _);
                    });
                }
                else
                {
                    if (s.Owner != null && s.Owner.UserId == this.Context.UserIdentifier)
                    {
                        s.Owner = s.Players.Any(p => !s.Disconnected.TryGetValue(p.UserId, out _)) ? s.Players.Where(p => !s.Disconnected.TryGetValue(p.UserId, out _)).First() : null;
                        if (s.Owner != null)
                        {
                            this.Clients.User(s.Owner.UserId).SendAsync("SetOwnership", true, true).Wait();
                        }
                    }
                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendChatMessage(Player sender, string message)
        {
            if (_store.Sessions.TryGetValue(sender.SessionIdHash, out var session))
            {
                var players = session.Players;
                var index = players.IndexOf(sender);
                if (index != -1)
                {
                    await this.Clients.Group(sender.SessionIdHash).SendAsync("ChatMessage", sender, message);
                }
            }
        }

        public async Task RaiseCard(Player player, Card card, string threat)
        {
            if (_store.Sessions.TryGetValue(player.SessionIdHash, out var session))
            {
                var players = session.Players;
                var index = players.IndexOf(player);
                if (index != -1)
                {
                    if (session.CurrentPlayer == player)
                    {
                        player = players[index];
                        var currentPlayerHand = player.Hand;
                        //if card is matching suit, or player has no card from current suit or trump
                        if (currentPlayerHand.Contains(card) && (currentPlayerHand.Cards.All(c => c.Suit != session.CurrentSuit) || card.Suit == session.CurrentSuit || card.Suit == Suit.ElevationOfPrivilege))
                        {
                            if (card.Value == Value._A && threat == "")
                            {
                                return;
                            }
                            currentPlayerHand.Remove(card);
                            session.CurrentlyPlayedCards.Add(card);
                            session.RaisedCard = card;

                            if (!session.RoundWinner.HasValue)
                            {
                                session.RoundWinner = (player, card);
                            }
                            else
                            {
                                var maxCard = session.CurrentlyPlayedCards.Where(c => c.Suit == session.CurrentSuit || c.Suit == Suit.ElevationOfPrivilege).Max();
                                if (maxCard != null && maxCard.Equals(card))
                                {
                                    session.RoundWinner = (player, card);
                                }
                            }
                            if (threat != "")
                            {
                                var key = new Issue(card, index, threat, players.IndexOf(session.CurrentPlayer));
                                session.Issues.TryAdd(key, new object());
                                await this.Clients.User(session.Owner.UserId).SendAsync("DecideIfLegit", key);
                                await this.Clients.Group(player.SessionIdHash).SendAsync("IdentifiedThreat", key);
                            }
                            session.Moves.Enqueue(new Move() { MoveType = MoveType.Raise, Card = session.RaisedCard, Player = player.NameEncrypted, Comment = threat, Round = session.Round });
                            await this.Clients.Caller.SendAsync("RaisedCardPrivate", player, card);
                            await this.Clients.Group(player.SessionIdHash).SendAsync("RaisedCard", player.NameEncrypted, card);
                            session.Riffing = true;
                            await this.Clients.Group(player.SessionIdHash).SendAsync("CurrentHighest", session.RoundWinner.Value.Item1.NameEncrypted, session.RoundWinner.Value.Item2);
                        }
                    }
                }
            }
        }

        public async Task EndGame(Player owner)
        {
            if (_store.Sessions.TryGetValue(owner.SessionIdHash, out var session))
            {
                if (owner == session.Owner)
                {
                    var players = session.Players;
                    var max = players.Max(p => p.Coins);
                    var winner = players.Find(p => p.Coins == max);
                    await this.Clients.Group(owner.SessionIdHash).SendAsync("GameEnded", winner.NameEncrypted);
                }
            }
        }

        public async Task AdvanceRound(Player owner)
        {
            if (_store.Sessions.TryGetValue(owner.SessionIdHash, out var session) && session.Players.Count > 0)
            {
                if (owner == session.Owner)
                {
                    await this.Clients.Group(owner.SessionIdHash).SendAsync("EndRiff");
                    session.Riffing = false;
                    var players = session.Players;
                    var index = players.IndexOf(session.CurrentPlayer);

                    if (session.CurrentlyPlayedCards.Count == session.StillPlayingActively)
                    {
                        session.Round += 1;
                        session.StillPlayingActively = players.Where(p => p.Hand.Count != 0).Count();
                        var winner = session.RoundWinner.Value.Item1;
                        await this.Clients.Group(owner.SessionIdHash).SendAsync("Winner", winner.NameEncrypted, session.RoundWinner.Value.Item2);
                        await this.Clients.Group(owner.SessionIdHash).SendAsync("CurrentPlayer", players.IndexOf(winner));
                        await this.Clients.User(winner.UserId).SendAsync("WinnerPrivate");
                        session.SuitChooser = winner;
                        if (players.Any(p => p.Hand.Cards.Count != 0))
                        {
                            if (winner.Hand.Cards.Count != 0)
                            {
                                session.CurrentPlayer = winner;
                            }
                            else
                            {
                                Player nextplayer;
                                do
                                {
                                    nextplayer = players[(++index) % players.Count];
                                }
                                while (nextplayer.Hand.Cards.Count == 0);
                                session.CurrentPlayer = nextplayer;
                                await this.Clients.Group(owner.SessionIdHash).SendAsync("CurrentPlayer", index % players.Count);
                                await this.Clients.User(nextplayer.UserId).SendAsync("YourTurn");
                            }
                            session.RoundWinner = null;
                            session.CurrentlyPlayedCards.Clear();
                        }
                        else
                        {
                            var max = session.Players.Max(p => p.Coins);
                            winner = session.Players.Find(p => p.Coins == max);
                            await this.Clients.Group(owner.SessionIdHash).SendAsync("GameEnded", winner.NameEncrypted);
                        }
                    }
                    else
                    {
                        if (session.Players.Any(p => p.Hand.Cards.Count != 0))
                        {
                            Player nextplayer;
                            do
                            {
                                nextplayer = players[(++index) % players.Count];
                            }
                            while (nextplayer.Hand.Cards.Count == 0);
                            session.CurrentPlayer = nextplayer;
                            await this.Clients.Group(owner.SessionIdHash).SendAsync("CurrentPlayer", index % players.Count);
                            await this.Clients.User(nextplayer.UserId).SendAsync("YourTurn");
                        }
                        else
                        {
                            var max = session.Players.Max(p => p.Coins);
                            var winner = session.Players.Find(p => p.Coins == max);
                            await this.Clients.Group(owner.SessionIdHash).SendAsync("GameEnded", winner.NameEncrypted);
                        }
                    }
                }
            }
        }

        public async Task FinishAbruptly(Player owner)
        {
            if (_store.Sessions.TryGetValue(owner.SessionIdHash, out var session))
            {
                if (owner == session.Owner)
                {
                    var players = session.Players;
                    if (players.Count > 0)
                    {
                        var max = players.Max(p => p.Coins);
                        var winner = players.Find(p => p.Coins == max);
                        await this.Clients.Group(owner.SessionIdHash).SendAsync("GameEnded", winner.NameEncrypted);
                    }
                }
            }
        }

        public async Task CreateSession(Player owner, bool ownerPlaying)
        {
            var sess = new Session(owner.SessionIdHash);
            if (_store.Sessions.TryAdd(sess.IDHash, sess))
            {
                var players = _store.Sessions[sess.IDHash].Players;
                if (ownerPlaying)
                {
                    players.Add(owner);
                    sess.OwnerPlaying = true;
                }
                owner.UserId = this.Context.UserIdentifier;
                owner.SessionIdHash = sess.IDHash;
                sess.Owner = owner;
                sess.OriginalOwner = owner;
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, sess.IDHash);
                await this.Clients.Caller.SendAsync("CreatedSessionPrivate", owner, sess.IDHash);
            }
        }

        public void LockSession(Player player)
        {
            if (_store.Sessions.TryGetValue(player.SessionIdHash, out var session))
            {
                if (session.Owner == player)
                {
                    session.Locked = true;
                }
            }
        }

        public async Task Export(Player player)
        {
            if (_store.Sessions.TryGetValue(player.SessionIdHash, out var session))
            {
                if (session.Owner == player)
                {
                    await this.Clients.Caller.SendAsync("Exported", session.Export());
                }
            }
        }

        public async Task JoinSession(Player player)
        {
            if (_store.Sessions.TryGetValue(player.SessionIdHash, out var session))
            {
                player.UserId = this.Context.UserIdentifier;
                if (session.Locked) { return; }
                if (session.Started)
                {
                    if (!session.Spectators.Contains(player))
                    {
                        session.Spectators.Add(player);
                    }
                    await this.Groups.AddToGroupAsync(this.Context.ConnectionId, player.SessionIdHash);
                    await this.Clients.Caller.SendAsync("SpectatingPrivate", player, session.GetPublicData());
                    await this.Clients.Group(player.SessionIdHash).SendAsync("CurrentPlayer", session.Players.IndexOf(session.CurrentPlayer));
                    await this.Clients.GroupExcept(player.SessionIdHash, this.Context.ConnectionId).SendAsync("Spectating", player.NameEncrypted);
                    return;
                }
                var players = session.Players;
                if (players.Contains(player))
                {
                    return;
                }
                players.Add(player);
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, player.SessionIdHash);
                await this.Clients.Caller.SendAsync("JoinedSessionPrivate", player, session.Players.Select(p => p.NameEncrypted), session.Spectators.Select(p => p.NameEncrypted));
                await this.Clients.GroupExcept(player.SessionIdHash, this.Context.ConnectionId).SendAsync("JoinedSession", player.NameEncrypted);
            }
        }

        public async Task TryRecover(Player p, string userid)
        {
            if (_store.Sessions.TryGetValue(p.SessionIdHash, out var session))
            {
                var player = session.Players.Any(e => e.UserId == userid) ? session.Players.Where(e => e.UserId == userid).First() : null;
                if (player != null || session.OriginalOwner.UserId == userid)
                {
                    player ??= session.OriginalOwner; // when owner is not playing
                    session.Disconnected.TryRemove(player.UserId, out _);
                    session.Spectators.Remove(p);
                    await this.Clients.Caller.SendAsync("Recovered", player);
                    if (session.OriginalOwner == player)
                    {
                        if (!(session.Owner is null))
                        {
                            await this.Clients.User(session.Owner.UserId).SendAsync("SetOwnership", false, true);
                        }

                        session.Owner = player;
                        await this.Clients.User(session.Owner.UserId).SendAsync("SetOwnership", true, session.OwnerPlaying);

                    }
                    if (session.RoundWinner.HasValue)
                    {
                        await this.Clients.Caller.SendAsync("CurrentHighest", session.RoundWinner.Value.Item1.NameEncrypted, session.RoundWinner.Value.Item2);
                    }
                    await this.Clients.Caller.SendAsync("ChangedSuit", player.NameEncrypted, session.CurrentSuit, session.ChosenSuits.ToArray());
                    this.Clients.Caller.SendAsync("CurrentPlayer", session.Players.IndexOf(session.CurrentPlayer)).Wait();
                    if (player == session.CurrentPlayer && !session.Riffing)
                    {
                        await this.Clients.Caller.SendAsync("CurrentPlayer", session.Players.IndexOf(player));
                        await this.Clients.Caller.SendAsync("YourTurn");
                    }

                    if (session.SuitChooser == player)
                    {
                        await this.Clients.Caller.SendAsync("WinnerPrivate");
                    }
                    foreach (var i in session.Issues)
                    {
                        if (session.Owner == player)
                        {
                            await this.Clients.Caller.SendAsync("DecideIfLegit", i.Key);
                        }
                    }
                }
            }
        }

        public async Task StartSession(Player owner)
        {
            if (_store.Sessions.TryGetValue(owner.SessionIdHash, out var session))
            {
                if (session.Owner == owner)
                {
                    if (!session.OwnerPlaying)
                    {
                        await this.Clients.User(owner.UserId).SendAsync("StartedSessionPrivate", new Hand());
                    }
                    session.Start();
                    for (var i = 0; i < session.Players.Count; i++)
                    {
                        var player = session.Players[i];
                        await this.Clients.User(player.UserId).SendAsync("StartedSessionPrivate", player.Hand);
                        if (player.First)
                        {
                            session.CurrentPlayer = player;
                            await this.Clients.Group(owner.SessionIdHash).SendAsync("CurrentPlayer", i % session.Players.Count);
                            await this.Clients.User(player.UserId).SendAsync("YourTurn");
                        }
                    }
                }
            }
        }

        public async Task IdentifyThreat(Player player, string threat)
        {
            if (_store.Sessions.TryGetValue(player.SessionIdHash, out var session))
            {
                var players = session.Players;
                var index = players.IndexOf(player);
                if (index != -1)
                {
                    var key = new Issue(session.RaisedCard, index, threat, players.IndexOf(session.CurrentPlayer));
                    session.Issues.TryAdd(key, new object());
                    session.Moves.Enqueue(new Move() { MoveType = MoveType.Riff, Card = session.RaisedCard, Player = player.NameEncrypted, Comment = threat, Round = session.Round });
                    await this.Clients.Group(player.SessionIdHash).SendAsync("IdentifiedThreat", key);
                    await this.Clients.User(session.Owner.UserId).SendAsync("DecideIfLegit", key);
                }
            }
        }

        public async Task AwardThreat(Player owner, Issue i, bool award)
        {
            if (_store.Sessions.TryGetValue(owner.SessionIdHash, out var session))
            {
                if (owner == session.Owner)
                {
                    var index = i.Player;
                    var currentindex = i.OriginalPlayer;
                    if (index > -1 && index < session.Players.Count)
                    {
                        var player = session.Players[index];
                        var coins = 0;
                        if (award)
                        {
                            coins++;
                            if (currentindex == index)
                            {
                                coins++;
                            }
                        }
                        player.Coins += coins;
                        session.Moves.Enqueue(new Move() { MoveType = MoveType.Award, Coins = coins, Card = i.Card, Player = player.NameEncrypted, Comment = i.EncryptedThreat, Round = session.Round });
                        session.Issues.TryRemove(i, out _);
                        session.AwardedIssues.TryAdd(i, new object());
                        await this.Clients.User(player.UserId).SendAsync("AwardedThreat", player, coins);
                    }
                }
            }
        }

        public async Task AwardThreatSimplified(Player owner, int index, string EncryptedThreat)
        {
            if (_store.Sessions.TryGetValue(owner.SessionIdHash, out var session))
            {
                if (owner == session.Owner)
                {
                    if (index > -1 && index < session.Players.Count)
                    {
                        var player = session.Players[index];
                        var coins = session.Players.IndexOf(session.CurrentPlayer) == index ? 2 : 1;
                        player.Coins += coins;
                        session.Moves.Enqueue(new Move() { MoveType = MoveType.Award, Coins = coins, Card = session.RaisedCard, Player = player.NameEncrypted, Comment = EncryptedThreat, Round = session.Round });
                        await this.Clients.User(player.UserId).SendAsync("AwardedThreat", player, coins);
                    }
                }
            }
        }

        public async Task ChangeSuit(Player player, Suit suit)
        {
            if (_store.Sessions.TryGetValue(player.SessionIdHash, out var session))
            {
                var players = session.Players;
                var index = players.IndexOf(player);
                if (index != -1 && session.SuitChooser == player)
                {
                    session.SuitChooser = null;
                    session.CurrentSuit = suit;
                    session.ChosenSuits.Add(suit);
                    await this.Clients.Group(player.SessionIdHash).SendAsync("ChangedSuit", player.NameEncrypted, suit, session.ChosenSuits.ToArray());
                    var nextplayer = session.CurrentPlayer;
                    if (nextplayer.Hand.Cards.Count == 0)
                    {
                        do
                        {
                            nextplayer = players[(index + 1) % players.Count];
                        }
                        while (nextplayer.Hand.Cards.Count == 0);
                        session.CurrentPlayer = nextplayer;
                    }
                    await this.Clients.Group(player.SessionIdHash).SendAsync("CurrentPlayer", index % players.Count);
                    await this.Clients.User(nextplayer.UserId).SendAsync("YourTurn");
                    session.Moves.Enqueue(new Move() { MoveType = MoveType.ChooseSuit, Comment = suit.ToString(), Round = session.Round, Player = player.NameEncrypted });

                }
            }
        }

        public async Task ForceSuit(Player owner, Suit suit)
        {
            if (_store.Sessions.TryGetValue(owner.SessionIdHash, out var session))
            {
                if (owner == session.Owner)
                {
                    session.CurrentSuit = suit;
                    session.ChosenSuits.Add(suit);
                    await this.Clients.Group(owner.SessionIdHash).SendAsync("ChangedSuit", owner.NameEncrypted, suit, session.ChosenSuits.ToArray());
                    if (session.SuitChooser != null)
                    {
                        await this.Clients.User(session.SuitChooser.UserId).SendAsync("YourTurn");
                        session.SuitChooser = null;
                    }
                    session.Moves.Enqueue(new Move() { MoveType = MoveType.ForceSuit, Comment = suit.ToString(), Round = session.Round });
                }
            }
        }
    }
}
