using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Combinations
{
    internal class Combination : IComparable
    {
        public Combination(HandType type, List<Data.Card> cards, Data.Card kicker)
        {
            Cards = cards;
            Type = type;
            Kicker = kicker;
        }

        public Combination(HandType type, List<Data.Card> cards)
        {
            Cards = cards;
            Type = type;
        }

        public HandType Type { get; }

        public List<Data.Card> Cards { get; }

        public Data.Card Kicker { get; }


        public int CompareTo(object other)
        {
            return CombComparer.CompareTo(this, (Combination) other);
        }

        public override string ToString()
        {
            var s = "";

            s += Type + ": ";

            s = Cards.Aggregate(s, (current, card) => current + (card + ", "));

            return s.Substring(0, s.Length - 2);
        }
    }
}