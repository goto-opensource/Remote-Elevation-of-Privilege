using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace rEoP.Shared.Model
{
    /// <summary>
    /// Always a new, full deck.
    /// Subsets are called "Hand"s
    /// </summary>
    public class Deck : IEnumerable<Card>
    {
        private readonly List<Card> cards = new List<Card> {
            new Card(Suit.Tampering, Value._3),
            new Card(Suit.Tampering, Value._4),
            new Card(Suit.Tampering, Value._5),
            new Card(Suit.Tampering, Value._6),
            new Card(Suit.Tampering, Value._7),
            new Card(Suit.Tampering, Value._8),
            new Card(Suit.Tampering, Value._9),
            new Card(Suit.Tampering, Value._10),
            new Card(Suit.Tampering, Value._J),
            new Card(Suit.Tampering, Value._Q),
            new Card(Suit.Tampering, Value._K),
            new Card(Suit.Tampering, Value._A),

            new Card(Suit.Spoofing, Value._2),
            new Card(Suit.Spoofing, Value._3),
            new Card(Suit.Spoofing, Value._4),
            new Card(Suit.Spoofing, Value._5),
            new Card(Suit.Spoofing, Value._6),
            new Card(Suit.Spoofing, Value._7),
            new Card(Suit.Spoofing, Value._8),
            new Card(Suit.Spoofing, Value._9),
            new Card(Suit.Spoofing, Value._10),
            new Card(Suit.Spoofing, Value._J),
            new Card(Suit.Spoofing, Value._Q),
            new Card(Suit.Spoofing, Value._K),
            new Card(Suit.Spoofing, Value._A),

            new Card(Suit.Repudiation, Value._2),
            new Card(Suit.Repudiation, Value._3),
            new Card(Suit.Repudiation, Value._4),
            new Card(Suit.Repudiation, Value._5),
            new Card(Suit.Repudiation, Value._6),
            new Card(Suit.Repudiation, Value._7),
            new Card(Suit.Repudiation, Value._8),
            new Card(Suit.Repudiation, Value._9),
            new Card(Suit.Repudiation, Value._10),
            new Card(Suit.Repudiation, Value._J),
            new Card(Suit.Repudiation, Value._Q),
            new Card(Suit.Repudiation, Value._K),
            new Card(Suit.Repudiation, Value._A),

            new Card(Suit.InformationDisclosure, Value._2),
            new Card(Suit.InformationDisclosure, Value._3),
            new Card(Suit.InformationDisclosure, Value._4),
            new Card(Suit.InformationDisclosure, Value._5),
            new Card(Suit.InformationDisclosure, Value._6),
            new Card(Suit.InformationDisclosure, Value._7),
            new Card(Suit.InformationDisclosure, Value._8),
            new Card(Suit.InformationDisclosure, Value._9),
            new Card(Suit.InformationDisclosure, Value._10),
            new Card(Suit.InformationDisclosure, Value._J),
            new Card(Suit.InformationDisclosure, Value._Q),
            new Card(Suit.InformationDisclosure, Value._K),
            new Card(Suit.InformationDisclosure, Value._A),

            new Card(Suit.DenialOfService, Value._2),
            new Card(Suit.DenialOfService, Value._3),
            new Card(Suit.DenialOfService, Value._4),
            new Card(Suit.DenialOfService, Value._5),
            new Card(Suit.DenialOfService, Value._6),
            new Card(Suit.DenialOfService, Value._7),
            new Card(Suit.DenialOfService, Value._8),
            new Card(Suit.DenialOfService, Value._9),
            new Card(Suit.DenialOfService, Value._10),
            new Card(Suit.DenialOfService, Value._J),
            new Card(Suit.DenialOfService, Value._Q),
            new Card(Suit.DenialOfService, Value._K),
            new Card(Suit.DenialOfService, Value._A),

            new Card(Suit.ElevationOfPrivilege, Value._5),
            new Card(Suit.ElevationOfPrivilege, Value._6),
            new Card(Suit.ElevationOfPrivilege, Value._7),
            new Card(Suit.ElevationOfPrivilege, Value._8),
            new Card(Suit.ElevationOfPrivilege, Value._9),
            new Card(Suit.ElevationOfPrivilege, Value._10),
            new Card(Suit.ElevationOfPrivilege, Value._J),
            new Card(Suit.ElevationOfPrivilege, Value._Q),
            new Card(Suit.ElevationOfPrivilege, Value._K),
            new Card(Suit.ElevationOfPrivilege, Value._A),

            new Card(Suit.Privacy, Value._2),
            new Card(Suit.Privacy, Value._3),
            new Card(Suit.Privacy, Value._4),
            new Card(Suit.Privacy, Value._5),
            new Card(Suit.Privacy, Value._6),
            new Card(Suit.Privacy, Value._7),
            new Card(Suit.Privacy, Value._8),
            new Card(Suit.Privacy, Value._9),
            new Card(Suit.Privacy, Value._10),
            new Card(Suit.Privacy, Value._J),
            new Card(Suit.Privacy, Value._Q),
            new Card(Suit.Privacy, Value._K),
            new Card(Suit.Privacy, Value._A)
        };

        public int Count => this.cards.Count;

        public void Remove(Card c)
        {
            this.cards.Remove(c);
        }

        public Card this[int index] => this.cards[index];

        public Deck(bool shuffled = false)
        {
            if (shuffled)
            {
                this.Shuffle();
            }
        }

        public void Shuffle()
        {
            using var rnd = new RNGCryptoServiceProvider();
            var buf = new byte[2];
            for (var i = this.Count; i > 0; i--)
            {
                rnd.GetBytes(buf);
                this.Swap(buf[0] % this.cards.Count, buf[1] % this.cards.Count);
            }
        }

        private void Swap(int i, int j)
        {
            var temp = this.cards[i];
            this.cards[i] = this.cards[j];
            this.cards[j] = temp;
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return this.cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.cards.GetEnumerator();
        }
    }
}
