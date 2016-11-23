using System;
using System.Collections.Generic;

namespace Bot.Combinations
{
    class Combination : IComparable
    {
        private HandType type;
        private List<Card> cards;
        private Card kicker;

        public Combination(HandType type, List<Card> cards, Card kicker)
        {
            this.cards = cards;
            this.type = type;
            this.kicker = kicker;
        }

        public Combination(HandType type, List<Card> cards)
        {
            this.cards = cards;
            this.type = type;
        }

        public HandType Type
        {
            get
            {
                return type;
            }
        }

        public List<Card> Cards
        {

            get
            {
                return cards;
            }
        }

        public Card Kicker
        {
            get
            {
                return kicker;
            }
        }



        public int CompareTo(object other)
        {
            return CombComparer.compareTo(this, (Combination)other);
        }

        public override string ToString()
        {
            string s = "";

            s += type.ToString() + ": ";

            foreach(Card card in cards)
            {
                s += card.ToString() + ", ";
            }

            return s.Substring(0, s.Length - 2);
        }




    }
}
