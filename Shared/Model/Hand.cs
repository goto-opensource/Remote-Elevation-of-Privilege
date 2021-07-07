using System;
using System.Collections.Generic;
using System.Linq;

namespace rEoP.Shared.Model
{
    public class Hand
    {
        public List<Card> Cards { get; set; }

        public Hand()
        {
            this.Cards = new List<Card>();
        }

        public Hand(List<Card> cards)
        {
            this.Cards = cards;
        }

        public void Add(Card card)
        {
            this.Cards.Add(card);
        }
        public int Count => this.Cards.Count;

        public Card this[int key]
        {
            get => this.Cards[key];
        }

        public bool HasOnlyAceForSuit(Suit suit)
        {
            return Cards.Where(c => c.Suit == suit).All(c => c.Value == Value._A);
        }

        public void Remove(Card card)
        {
            this.Cards.Remove(card);
        }

        public bool Contains(Card card)
        {
            return this.Cards.Contains(card);
        }

        public void SortBySuit(Suit s)
        {
            if (s == Suit.ElevationOfPrivilege)
            {
                this.Cards = this.Cards.Where(c => c.Suit == s).Concat(this.Cards.Where(c => c.Suit != s)).ToList();
            }
            else
            {
                this.Cards = this.Cards.Where(c => c.Suit == s).Concat(this.Cards.Where(c => c.Suit == Suit.ElevationOfPrivilege).Concat(this.Cards.Where(c => c.Suit != s && c.Suit != Suit.ElevationOfPrivilege))).ToList();
            }

        }
    }
}
