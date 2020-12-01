using System;

namespace rEoP.Shared.Model
{
    public class Card : IComparable<Card>
    {
        public Card() { }

        public Card(Suit suit, Value value)
        {
            this.Suit = suit;
            this.Value = value;
        }

        public Suit Suit { get; set; }
        public Value Value { get; set; }

        public int CompareTo(Card other) //Can't link to current Suit! Check on caller side!
        {
            if (this.Suit != other.Suit)
            {
                if (this.Suit == Suit.ElevationOfPrivilege)
                {
                    return 1;
                }
                if (other.Suit == Suit.ElevationOfPrivilege)
                {
                    return -1;
                }
                else
                {
                    throw new ArgumentException("We don't do that here (Comparison of different suits (not EoP)).");
                }
            }
            return this.Value - other.Value;

        }

        public override bool Equals(object obj)
        {
            return obj is Card card &&
                   this.Suit == card.Suit &&
                   this.Value == card.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Suit, this.Value);
        }

        public override string ToString()
        {
            return this.Suit.ToString() + this.Value.ToString();
        }
    }
}
