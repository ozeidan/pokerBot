using System;

namespace Bot.Data
{
    public enum Suit
    {
        Herz,
        Karo,
        Kreuz,
        Pik,
        Error
    }

    public enum Value
    {
        Zwei,
        Drei,
        Vier,
        Fünf,
        Sechs,
        Sieben,
        Acht,
        Neun,
        Zehn,
        Bube,
        Dame,
        König,
        Ass,
        Error
    }

    internal class Card : IComparable
    {
        // 0 - 12 -> Ass, 2, ..., 10, Bube, Dame, König
        // 0 - 3 -> Herz Karo Kreuz Pik


        //You can intizialize the card with the enumerators...
        public Card(Suit suit, Value value)
        {
            Suit = suit;
            Value = value;
        }


        //Or a string according to the recource files
        public Card(string name)
        {
            try
            {
                if (name.Length != 3)
                    throw new ArgumentException();

                var c = name[1];
                int i;

                switch (c)
                {
                    case 'E':
                        i = 0;
                        break;
                    case 'A':
                        i = 1;
                        break;
                    case 'R':
                        i = 2;
                        break;
                    case 'I':
                        i = 3;
                        break;
                    default:
                        throw new ArgumentException();
                }

                Suit = (Suit) i;

                c = name[2];

                switch (c)
                {
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        i = (int) char.GetNumericValue(c) - 2;
                        break;
                    case '1':
                        i = 8;
                        break;
                    case 'J':
                        i = 9;
                        break;
                    case 'Q':
                        i = 10;
                        break;
                    case 'K':
                        i = 11;
                        break;
                    case 'A':
                        i = 12;
                        break;
                    default:
                        throw new ArgumentException();
                }

                Value = (Value) i;
            }
            catch (ArgumentException)
            {
                Value = Value.Error;
                Suit = Suit.Error;
            }
        }

        public Value Value { get; }

        public Suit Suit { get; }

        public int CompareTo(object obj)
        {
            var compareCard = (Card) obj;

            if (compareCard.Value > Value)
                return -1;
            if (compareCard.Value < Value)
                return 1;
            return 0;
        }


        public override string ToString()
        {
            return Suit + " " + Value;
        }
    }
}