using System;

namespace rEoP.Shared.Model
{
    public class Player
    {
        public string UserId { get; set; }
        public string SessionIdHash { get; set; }
        public int Coins { get; set; } = 0;
        public Hand Hand { get; set; } = new Hand();
        public string NameEncrypted { get; set; }
        public Card Raise(Card card)
        {
            this.Hand.Remove(card);
            return card;
        }

        public bool First => this.Hand.Contains(new Card(Suit.Tampering, Value._3));

        public override bool Equals(object obj)
        {
            return obj is Player player && this.UserId.SafeEquals(player.UserId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.NameEncrypted, this.UserId);
        }

        public void DealCard(Card c)
        {
            this.Hand.Add(c);
        }

        public void UpdateHand(Hand hand)
        {
            this.Hand = hand;
        }

        public static bool operator ==(Player left, Player right)
        {
            if (left is null && !(right is null))
            {
                return false;
            }

            if (left is null && right is null)
            {
                return true;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Player left, Player right)
        {
            return !(left == right);
        }
    }
}
