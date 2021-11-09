using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace rEoP.Shared.Model
{
    // replace concurrency with list of increments on the session state
    public class Session
    {
        public int Round { get; set; } = 0;
        public Deck Deck { get; }
        public List<Player> Players { get; } = new List<Player>(10);
        public List<Player> Spectators { get; } = new List<Player>(10);
        public ConcurrentQueue<Move> Moves { get; } = new ConcurrentQueue<Move>();
        public string Whiteboard { get; set; }
        public Player Winner { get; }
        public Player SuitChooser { get; set; }
        public Card RaisedCard { get; set; }
        public string IDHash { get; }
        /// <summary>
        /// Current Suit a player has to play if having it. Tampering 3 starts the game, so Tampering is the starting suit.
        /// </summary>
        public Suit CurrentSuit { get; set; } = Suit.Tampering;
        public Player CurrentPlayer { get; set; }
        public bool Started { get; set; } = false;
        public bool Riffing { get; set; } = false;
        public (Player, Card)? RoundWinner { get; set; }
        public HashSet<Suit> ChosenSuits { get; } = new HashSet<Suit>() { Suit.Tampering };
        public HashSet<Card> CurrentlyPlayedCards { get; } = new HashSet<Card>();
        public Player Owner { get; set; }
        public Player OriginalOwner { get; set; }

        [JsonConverter(typeof(ConcurrentDictionaryConverter<Issue>))]
        public ConcurrentDictionary<Issue, object> Issues { get; } = new ConcurrentDictionary<Issue, object>();

        [JsonConverter(typeof(ConcurrentDictionaryConverter<Issue>))]
        public ConcurrentDictionary<Issue, object> AwardedIssues { get; } = new ConcurrentDictionary<Issue, object>();
        public bool Locked { get; set; }

        [JsonConverter(typeof(ConcurrentDictionaryConverter<string>))]
        public ConcurrentDictionary<string, object> Disconnected { get; } = new ConcurrentDictionary<string, object>();
        public int StillPlayingActively { get; set; }
        public bool OwnerPlaying { get; set; } = false;

        public Session(string idHash, Deck.DeckType type)
        {
            this.IDHash = idHash;
            this.Deck = new Deck(type);
            this.Deck.Shuffle();
        }

        public bool Join(Player player)
        {
            lock (this.Players)
            {
                if (this.Started)
                {
                    return false;
                }

                if (this.Players.Contains(player))
                {
                    return false;
                }

                this.Players.Add(player);
                this.StillPlayingActively++;
                return true;
            }
        }

        public SessionPublicData GetPublicData()
        {
            return new SessionPublicData(
                this.Players.Select(p => p.NameEncrypted).ToList(),
                this.Spectators.Select(p => p.NameEncrypted).ToList(),
                this.Riffing,
                this.CurrentlyPlayedCards,
                this.CurrentPlayer,
                this.CurrentSuit,
                this.RoundWinner,
                this.RaisedCard,
                this.Issues,
                this.Deck.Type
            );
        }

        public void Start()
        {
            if (this.Started)
            {
                return;
            }
            if (this.Players.Count == 0) return;
            for (int i = 0, j = this.Deck.Count - 1; this.Deck.Count != 0; i++, j--)
            {
                this.Players[i % this.Players.Count].DealCard(this.Deck[j]);
                this.Deck.Remove(this.Deck[j]);
            }
            this.Started = true;
            this.StillPlayingActively = this.Players.Count;
        }

        public string Export()
        {
            return JsonSerializer.Serialize(this.Moves);
        }

        public override bool Equals(object obj)
        {
            return obj is Session session &&
                   this.IDHash.SafeEquals(session.IDHash);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.IDHash);
        }

        public static bool operator ==(Session left, Session right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Session left, Session right)
        {
            return !(left == right);
        }
    }
}
