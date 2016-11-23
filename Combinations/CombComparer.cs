using System.Collections.Generic;
using System.Linq;

namespace Bot.Combinations
{
    class CombComparer
    {
        public static int compareTo(Combination firstPair, Combination secondPair)
        {
            if (firstPair.Type.CompareTo(secondPair.Type) != 0)
            {
                return firstPair.Type.CompareTo(secondPair.Type);
            }
            else
            {
                firstPair.Cards.Sort();
                secondPair.Cards.Sort();

                switch (firstPair.Type)
                {
                    case HandType.HighCard:
                    case HandType.OnePair:
                    case HandType.ThreeofaKind:
                    case HandType.FourofaKind:
                        int compareVal = firstPair.Cards.First().CompareTo(secondPair.Cards.First());

                        if (compareVal != 0)
                            return compareVal;
                        else
                            return firstPair.Kicker.CompareTo(secondPair.Kicker);

                    case HandType.Straight:
                    case HandType.StraightFlush:

                        return firstPair.Cards.First().CompareTo(secondPair.Cards.First());

                    case HandType.TwoPair:

                        //Bei zwei Twopairs werden erst die zwei höheren Karten verglichen, dann die zwei niedrigeren Karten
                        int firstComp = firstPair.Cards.Last().CompareTo(secondPair.Cards.Last());
                        if (firstComp == 0)
                        {
                            int secondComp = firstPair.Cards.First().CompareTo(secondPair.Cards.First());
                            if (secondComp == 0)
                                return firstPair.Kicker.CompareTo(secondPair.Kicker);
                            else
                                return secondComp;
                        }
                        else
                            return firstComp;

                    case HandType.Flush:

                        int firstSum = sumCardValues(firstPair.Cards);
                        int secondSum = sumCardValues(secondPair.Cards);
                        return firstSum.CompareTo(secondSum);

                    case HandType.RoyalFlush:
                        return 0;

                    case HandType.FullHouse:
                        return evalFullHouse(firstPair.Cards, secondPair.Cards);
                    default:
                        throw new MyExceptions.CompareExcepion();

                }

            }

        }

        private static int sumCardValues(List<Card> cards)
        {
            int count = 0;

            foreach (Card card in cards)
            {
                count += (int)card.Value;
            }

            return count;
        }

        private static int evalFullHouse(List<Card> first, List<Card> second)
        {
            //Herausfinden welcher Karten das Threeofakind sind und welche das Pair
            //Danach erst Threeofakind Rang vergleichen, und falls diese gleich sind das Pair
            //Karten sind aufsteigend sortiert

            Card firstTriple = firstThreeOfAKind(first) ? first.First() : first.Last();
            Card secondTriple = firstThreeOfAKind(second) ? second.First() : second.Last();

            if (firstTriple.CompareTo(secondTriple) != 0)
            {
                return firstTriple.CompareTo(secondTriple);
            }
            else
            {
                Card firstDouble = !firstThreeOfAKind(first) ? first.First() : first.Last();
                Card secondDouble = !firstThreeOfAKind(second) ? second.First() : second.Last();

                return firstDouble.CompareTo(secondDouble);
            }


        }
        /// <summary>
        /// Calculates if the first or the last 3 cards from the three of a kind
        /// </summary>
        /// <param name="cards">The 5 cards which form a full house</param>
        /// <returns>True if the three of a kind are is at the beginning of the list, else false</returns>

        private static bool firstThreeOfAKind(List<Card> cards)
        {
            int value = (int)cards[0].Value;

            int count = 1;

            for (int i = 1; i < cards.Count; i++)
            {
                if (value == (int)cards[i].Value)
                {
                    count++;
                }
            }

            if (count == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
