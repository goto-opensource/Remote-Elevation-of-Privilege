using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace rEoP.Shared.Model
{
    public static class DeckTypeExtensions
    {
        public static Suit[] GetSuits(this Deck.DeckType deckType)
        {
            switch (deckType)
            {
                case Deck.DeckType.Cornucopia:
                    return new[] { Suit.DataValidationAndEncoding, Suit.Authentication, Suit.SessionManagement, Suit.Authorization, Suit.Cryptography, Suit.Cornucopia };
                case Deck.DeckType.REOP:
                default:
                    return new[] { Suit.Spoofing, Suit.Tampering, Suit.Repudiation, Suit.InformationDisclosure, Suit.DenialOfService, Suit.ElevationOfPrivilege, Suit.Privacy };
            }
        }

        public static Suit GetTrumpSuit(this Deck.DeckType deckType)
        {
            switch (deckType)
            {
                case Deck.DeckType.Cornucopia:
                    return Suit.Cornucopia;
                case Deck.DeckType.REOP:
                default:
                    return Suit.ElevationOfPrivilege;
            }
        }
    }

    /// <summary>
    /// Always a new, full deck.
    /// Subsets are called "Hand"s
    /// </summary>
    public class Deck : IEnumerable<Card>
    {

        public enum DeckType
        {
            REOP, Cornucopia
        }

        private readonly List<Card> cards;

        public Suit TrumpSuit { get; }

        public int Count => this.cards.Count;

        public DeckType Type { get; }

        public void Remove(Card c)
        {
            this.cards.Remove(c);
        }

        public Card this[int index] => this.cards[index];

        public Deck(DeckType deckType, bool shuffled = false)
        {
            this.Type = deckType;
            switch (deckType)
            {
                case DeckType.Cornucopia:
                    cards = new List<Card> {
                        new Card(Suit.DataValidationAndEncoding, Value._2),
                        new Card(Suit.DataValidationAndEncoding, Value._3),
                        new Card(Suit.DataValidationAndEncoding, Value._4),
                        new Card(Suit.DataValidationAndEncoding, Value._5),
                        new Card(Suit.DataValidationAndEncoding, Value._6),
                        new Card(Suit.DataValidationAndEncoding, Value._7),
                        new Card(Suit.DataValidationAndEncoding, Value._8),
                        new Card(Suit.DataValidationAndEncoding, Value._9),
                        new Card(Suit.DataValidationAndEncoding, Value._10),
                        new Card(Suit.DataValidationAndEncoding, Value._J),
                        new Card(Suit.DataValidationAndEncoding, Value._Q),
                        new Card(Suit.DataValidationAndEncoding, Value._K),
                        new Card(Suit.DataValidationAndEncoding, Value._A),

                        new Card(Suit.Authentication, Value._2),
                        new Card(Suit.Authentication, Value._3),
                        new Card(Suit.Authentication, Value._4),
                        new Card(Suit.Authentication, Value._5),
                        new Card(Suit.Authentication, Value._6),
                        new Card(Suit.Authentication, Value._7),
                        new Card(Suit.Authentication, Value._8),
                        new Card(Suit.Authentication, Value._9),
                        new Card(Suit.Authentication, Value._10),
                        new Card(Suit.Authentication, Value._J),
                        new Card(Suit.Authentication, Value._Q),
                        new Card(Suit.Authentication, Value._K),
                        new Card(Suit.Authentication, Value._A),

                        new Card(Suit.SessionManagement, Value._2),
                        new Card(Suit.SessionManagement, Value._3),
                        new Card(Suit.SessionManagement, Value._4),
                        new Card(Suit.SessionManagement, Value._5),
                        new Card(Suit.SessionManagement, Value._6),
                        new Card(Suit.SessionManagement, Value._7),
                        new Card(Suit.SessionManagement, Value._8),
                        new Card(Suit.SessionManagement, Value._9),
                        new Card(Suit.SessionManagement, Value._10),
                        new Card(Suit.SessionManagement, Value._J),
                        new Card(Suit.SessionManagement, Value._Q),
                        new Card(Suit.SessionManagement, Value._K),
                        new Card(Suit.SessionManagement, Value._A),

                        new Card(Suit.Authorization, Value._2),
                        new Card(Suit.Authorization, Value._3),
                        new Card(Suit.Authorization, Value._4),
                        new Card(Suit.Authorization, Value._5),
                        new Card(Suit.Authorization, Value._6),
                        new Card(Suit.Authorization, Value._7),
                        new Card(Suit.Authorization, Value._8),
                        new Card(Suit.Authorization, Value._9),
                        new Card(Suit.Authorization, Value._10),
                        new Card(Suit.Authorization, Value._J),
                        new Card(Suit.Authorization, Value._Q),
                        new Card(Suit.Authorization, Value._K),
                        new Card(Suit.Authorization, Value._A),

                        new Card(Suit.Cryptography, Value._2),
                        new Card(Suit.Cryptography, Value._3),
                        new Card(Suit.Cryptography, Value._4),
                        new Card(Suit.Cryptography, Value._5),
                        new Card(Suit.Cryptography, Value._6),
                        new Card(Suit.Cryptography, Value._7),
                        new Card(Suit.Cryptography, Value._8),
                        new Card(Suit.Cryptography, Value._9),
                        new Card(Suit.Cryptography, Value._10),
                        new Card(Suit.Cryptography, Value._J),
                        new Card(Suit.Cryptography, Value._Q),
                        new Card(Suit.Cryptography, Value._K),
                        new Card(Suit.Cryptography, Value._A),

                        new Card(Suit.Cornucopia, Value._2),
                        new Card(Suit.Cornucopia, Value._3),
                        new Card(Suit.Cornucopia, Value._4),
                        new Card(Suit.Cornucopia, Value._5),
                        new Card(Suit.Cornucopia, Value._6),
                        new Card(Suit.Cornucopia, Value._7),
                        new Card(Suit.Cornucopia, Value._8),
                        new Card(Suit.Cornucopia, Value._9),
                        new Card(Suit.Cornucopia, Value._10),
                        new Card(Suit.Cornucopia, Value._J),
                        new Card(Suit.Cornucopia, Value._Q),
                        new Card(Suit.Cornucopia, Value._K),
                        new Card(Suit.Cornucopia, Value._A)
                    };
                    TrumpSuit = Suit.Cornucopia;
                    break;
                case DeckType.REOP:
                default:
                    cards = new List<Card> {
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
                    TrumpSuit = Suit.ElevationOfPrivilege;
                    break;
            }
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
