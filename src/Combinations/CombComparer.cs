using System.Collections.Generic;
using System.Linq;
using Bot.Exceptions;

namespace Bot.Combinations
{
    internal class CombComparer
    {
        public static int CompareTo(Combination firstPair, Combination secondPair)
        {
            if (firstPair.Type.CompareTo(secondPair.Type) != 0)
                return firstPair.Type.CompareTo(secondPair.Type);
            firstPair.Cards.Sort();
            secondPair.Cards.Sort();

            switch (firstPair.Type)
            {
                case HandType.HighCard:
                case HandType.OnePair:
                case HandType.ThreeofaKind:
                case HandType.FourofaKind:
                    var compareVal = firstPair.Cards.First().CompareTo(secondPair.Cards.First());

                    return compareVal != 0 ? compareVal : firstPair.Kicker.CompareTo(secondPair.Kicker);

                case HandType.Straight:
                case HandType.StraightFlush:

                    return firstPair.Cards.First().CompareTo(secondPair.Cards.First());

                case HandType.TwoPair:

                    //Bei zwei Twopairs werden erst die zwei höheren Karten verglichen, dann die zwei niedrigeren Karten
                    var firstComp = firstPair.Cards.Last().CompareTo(secondPair.Cards.Last());
                    if (firstComp == 0)
                    {
                        var secondComp = firstPair.Cards.First().CompareTo(secondPair.Cards.First());
                        return secondComp == 0 ? firstPair.Kicker.CompareTo(secondPair.Kicker) : secondComp;
                    }
                    return firstComp;

                case HandType.Flush:

                    var firstSum = SumCardValues(firstPair.Cards);
                    var secondSum = SumCardValues(secondPair.Cards);
                    return firstSum.CompareTo(secondSum);

                case HandType.RoyalFlush:
                    return 0;

                case HandType.FullHouse:
                    return EvalFullHouse(firstPair.Cards, secondPair.Cards);
                default:
                    throw new CompareExcepion();
            }
        }

        private static int SumCardValues(List<Data.Card> cards)
        {
            return cards.Sum(card => (int) card.Value);
        }

        private static int EvalFullHouse(List<Data.Card> first, List<Data.Card> second)
        {
            //Herausfinden welcher Karten das Threeofakind sind und welche das Pair
            //Danach erst Threeofakind Rang vergleichen, und falls diese gleich sind das Pair
            //Karten sind aufsteigend sortiert

            var firstTriple = FirstThreeOfAKind(first) ? first.First() : first.Last();
            var secondTriple = FirstThreeOfAKind(second) ? second.First() : second.Last();

            if (firstTriple.CompareTo(secondTriple) != 0)
                return firstTriple.CompareTo(secondTriple);
            var firstDouble = !FirstThreeOfAKind(first) ? first.First() : first.Last();
            var secondDouble = !FirstThreeOfAKind(second) ? second.First() : second.Last();

            return firstDouble.CompareTo(secondDouble);
        }

        /// <summary>
        ///     Calculates if the first or the last 3 cards from the three of a kind
        /// </summary>
        /// <param name="cards">The 5 cards which form a full house</param>
        /// <returns>True if the three of a kind are is at the beginning of the list, else false</returns>
        private static bool FirstThreeOfAKind(List<Data.Card> cards)
        {
            var value = (int) cards[0].Value;

            var count = 1;

            for (var i = 1; i < cards.Count; i++)
                if (value == (int) cards[i].Value)
                    count++;

            return count == 3;
        }
    }
}