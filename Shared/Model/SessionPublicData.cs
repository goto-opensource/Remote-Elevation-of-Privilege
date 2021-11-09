using System.Collections.Concurrent;
using System.Collections.Generic;

namespace rEoP.Shared.Model
{
    public class SessionPublicData
    {
        public List<string> Players { get; set; }
        public List<string> Spectators { get; set; }
        public bool Riffing { get; set; }
        public HashSet<Card> CurrentlyPlayedCards { get; set; }
        public Player CurrentPlayer { get; set; }
        public Suit CurrentSuit { get; set; }
        public string RoundWinner { get; set; }
        public Card RoundWinnerCard { get; set; }
        public Card RaisedCard { get; set; }
        public Deck.DeckType DeckType { get; set; }
        [System.Text.Json.Serialization.JsonConverter(typeof(ConcurrentDictionaryConverter<Issue>))]
        public ConcurrentDictionary<Issue, object> Issues { get; set; }

        public SessionPublicData()
        {
        }

        public SessionPublicData(List<string> enumerable1, List<string> enumerable2, bool riffing, HashSet<Card> currentlyPlayedCards, Player currentPlayer, Suit currentSuit, (Player, Card)? roundWinner, Card raisedCard, ConcurrentDictionary<Issue, object> issues, Deck.DeckType deckType)
        {
            this.Players = enumerable1;
            this.Spectators = enumerable2;
            this.Riffing = riffing;
            this.CurrentlyPlayedCards = currentlyPlayedCards;
            this.CurrentPlayer = currentPlayer;
            this.CurrentSuit = currentSuit;
            if (roundWinner.HasValue)
            {
                this.RoundWinner = roundWinner.Value.Item1.NameEncrypted;
                this.RoundWinnerCard = roundWinner.Value.Item2;
            }
            this.RaisedCard = raisedCard;
            this.Issues = new ConcurrentDictionary<Issue, object>(issues);
            this.DeckType = deckType;
        }
    }
}