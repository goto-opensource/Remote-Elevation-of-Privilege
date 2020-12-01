using System;

namespace rEoP.Shared.Model
{
    public class Issue
    {
        public Card Card { get; set; }
        public int OriginalPlayer { get; set; }
        public int Player { get; set; }
        public string EncryptedThreat { get; set; }

        public Issue() { }

        public Issue(Card card, int playerIndex, string threatEncrypted, int originalPlayer)
        {
            this.Card = card;
            this.Player = playerIndex;
            this.EncryptedThreat = threatEncrypted;
            this.OriginalPlayer = originalPlayer;
        }

        public override bool Equals(object obj)
        {
            return obj is Issue issue &&
                   this.Card.Equals(issue.Card) &&
                   this.Player == issue.Player &&
                   this.EncryptedThreat == issue.EncryptedThreat;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Card, this.Player, this.EncryptedThreat);
        }
    }
}
