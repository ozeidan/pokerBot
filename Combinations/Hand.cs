using Bot.Combinations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot
{
    class MyHand : IComparable
    {
        SortedSet<Combination> combinations;
        Combination highest;

        public MyHand(List<Card> myCards, List<Card> tableCards)
        {
			myCards.AddRange(tableCards);

            combinations = CombDetector.detectCombinations(myCards);

            highest = combinations.Last();
        }

        public int CompareTo(object other)
        {
            return Combinations.CombComparer.compareTo(highest, ((MyHand)other).highest);
        }

        public Combination BestCombination
        {
            get
            {
                return highest;
            }
        }

        public override string ToString()
        {
            string s = "";

            foreach(Combination comb in combinations)
            {
                s += comb.ToString() + System.Environment.NewLine;
            }

            return s;
        }
    }
}
