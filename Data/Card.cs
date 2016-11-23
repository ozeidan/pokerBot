using System;

namespace Bot
{
    public enum Suit { Herz, Karo, Kreuz, Pik, Error };
    public enum Value
    {
        Zwei, Drei, Vier, Fünf, Sechs, Sieben, Acht,
        Neun, Zehn, Bube, Dame, König, Ass, Error
    };

    class Card : IComparable
    {
        // 0 - 3 -> Herz Karo Kreuz Pik
        private Suit suit;
        // 0 - 12 -> Ass, 2, ..., 10, Bube, Dame, König
        private Value cardValue;
        

        //You can intizialize the card with the enumerators...
        public Card(Suit suit, Value value)
        {
            this.suit = suit;
            this.cardValue = value;
        }


        //Or a string according to the recource files
        public Card(String name)
        {
            

            try
            {
                if (name.Length != 3)
                {
                    throw new ArgumentException();
                }

                char c = name[1];
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

                this.suit = (Suit)i;
                
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
                        i = ((int)Char.GetNumericValue(c)) - 2;
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

                this.cardValue = (Value)i;

                

            }
            catch (ArgumentException)
            {
                this.cardValue = Value.Error;
                this.suit = Suit.Error;
            }


        }




        public override string ToString()
        {
            return suit + " " + cardValue;

        }

        public int CompareTo(object obj)
        {
            Card compareCard = (Card)obj;

            if (compareCard.cardValue > this.cardValue)
            {
                return -1;
            }
            else if (compareCard.cardValue < this.cardValue)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public Value Value
        {
            get
            {
                return cardValue;
            }
        }

        public Suit Suit
        {
            get
            {
                return suit;
            }
        }
    }


}
