using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Combinations;

namespace Bot
{
    internal class MyHand : IComparable
    {
        private readonly SortedSet<Combination> _combinations;

        public MyHand(List<Data.Card> myCards, List<Data.Card> tableCards)
        {
            myCards.AddRange(tableCards);

            _combinations = CombDetector.DetectCombinations(myCards);

            BestCombination = _combinations.Last();
        }

        public Combination BestCombination { get; }

        public int CompareTo(object other)
        {
            return CombComparer.CompareTo(BestCombination, ((MyHand) other).BestCombination);
        }

        public override string ToString()
        {
            return _combinations.Aggregate("", (current, comb) => current + (comb + Environment.NewLine));
        }
    }
}