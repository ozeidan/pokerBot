using System.Collections.Generic;
using System.Linq;

namespace Bot.Combinations
{
    public enum HandType
    {
        HighCard, OnePair, TwoPair, ThreeofaKind, Straight,
        Flush, FullHouse, FourofaKind, StraightFlush, RoyalFlush, Error
    };

    static class CombDetector
    {

        private static SortedSet<Combination> combs;
        private static List<Card> cards;
        private static Dictionary<int, List<Card>> values;

        public static SortedSet<Combination> detectCombinations(List<Card> cards)
        {
            CombDetector.combs = new SortedSet<Combination>();
            CombDetector.cards = cards;

            //Höchste Karte wird zur High Card gemacht (findHighCard)
            //Zweithöchste Karte ist der Kicker
            cleanUp(cards);

            cards.Sort();

            findHighCard();

            initValueList();

            findDoublesTripels();

            findMultiples();

            if (findFlush(cards) != null)
            {
                combs.Add(new Combination(HandType.Flush, findFlush(cards)));
            }

            shiftValueList();

            return combs;
        }

        private static void cleanUp(List<Card> cards)
        {
            List<Card> removeList = new List<Card>();

            foreach (Card card in cards)
            {
                if (card.Value == Value.Error || card.Suit == Suit.Error)
                {
                    removeList.Add(card);
                }
            }

            foreach (Card card in removeList)
            {
                cards.Remove(card);
            }
        }


        private static void initValueList()
        {
            //Karten werden in Listen nach Wert unterteilt, damit sie besser ausgewertet werden können

            values = new Dictionary<int, List<Card>>();
            foreach (Card card in cards)
            {
                int value = (int)card.Value;

                if (!values.ContainsKey(value))
                {
                    List<Card> thiscard = new List<Card>();
                    thiscard.Add(card);
                    values.Add(value, thiscard);
                }
                else
                {
                    values[value].Add(card);
                }
            }
        }

        private static void findHighCard()
        {
            Card highCard = cards.Last();
            List<Card> oneCard = new List<Card>();
            oneCard.Add(highCard);

            if (cards.Count > 1)
            {
                combs.Add(new Combination(HandType.HighCard, oneCard, cards[cards.Count - 2]));
            }
            else
            {
                combs.Add(new Combination(HandType.HighCard, oneCard));
            }

        }


        private static void findDoublesTripels()
        {
            //Falls in value ein Eintrag eine Liste mit mehr als einem Eintrag ist gibt es ein Pair, TOK oder FOK
            foreach (int key in values.Keys)
            {
                if (values[key].Count > 1)
                {
                    List<Card> cardList = values[key];
                    Card kicker = findKicker(cards, cardList);
                    HandType type = HandType.Error;

                    int amount = cardList.Count;

                    switch (amount)
                    {
                        case 2:
                            type = HandType.OnePair;
                            break;
                        case 3:
                            type = HandType.ThreeofaKind;
                            break;
                        case 4:
                            type = HandType.FourofaKind;
                            break;
                    }

                    Combination pair = new Combination(type, cardList, kicker);
                    combs.Add(pair);
                }
            }
        }

        private static List<Card> findFlush(List<Card> cards)
        {
            Dictionary<Suit, List<Card>> suitDict = new Dictionary<Suit, List<Card>>();

            foreach (Card card in cards)
            {
                if (!suitDict.ContainsKey(card.Suit))
                {
                    suitDict[card.Suit] = new List<Card>();
                }

                suitDict[card.Suit].Add(card);
            }

            foreach (Suit key in suitDict.Keys)
            {
                if (suitDict[key].Count >= 5)
                {
                    return suitDict[key];
                }
            }

            return null;
        }

        private static void findMultiples()
        {
            List<Combination> doubleList = new List<Combination>();
            List<Combination> tripleList = new List<Combination>();

            foreach (Combination pair in combs)
            {
                if (pair.Type.Equals(HandType.OnePair))
                {
                    doubleList.Add(pair);
                }
                else if (pair.Type.Equals(HandType.ThreeofaKind))
                {
                    tripleList.Add(pair);
                }
            }

            if (tripleList.Count == 2)
            {
                List<Card> triples = tripleList[0].Cards;
                triples.AddRange(tripleList[1].Cards);

                triples.Sort();

                combs.Add(new Combination(HandType.FullHouse, triples.Skip(1).ToList()));
            }


            if (doubleList.Count > 0 && tripleList.Count > 0)
            {
                List<Card> doubles = new List<Card>();
                foreach (Combination pair in doubleList)
                {
                    doubles.AddRange(pair.Cards);
                }
                List<Card> triples = tripleList[0].Cards;

                triples.Sort();
                doubles.Sort();

                triples.AddRange(doubles.Take(2));

                combs.Add(new Combination(HandType.FullHouse, triples));
            }

            if (doubleList.Count > 1)
            {
                List<Card> cardList = new List<Card>();
                foreach (Combination pair in doubleList)
                {
                    cardList.AddRange(pair.Cards);
                }

                cardList.Sort();
                List<Card> twoPairCards = cardList.GetRange(cardList.Count - 4, 4);
                Card newKicker = findKicker(cards, twoPairCards);

                combs.Add(new Combination(HandType.TwoPair, twoPairCards, newKicker));
            }
        }


        private static void findStraight()
        {
            findStraight();

            int count = 0;

            for (int i = 0; i < 13; i++)
            {
                int amount = values.ContainsKey(i) ? values[i].Count : 0;

                if (amount > 0)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }

                if (count >= 5)
                {
                    List<Card> straight = new List<Card>();

                    for (int j = i - 4; j < i + 1; j++)
                    {
                        straight.Add(values[j][0]);
                    }

                    combs.Add(new Combination(HandType.Straight, straight));


                    if (findFlush(straight) != null)
                    {
                        combs.Add(new Combination(HandType.StraightFlush, straight));

                        if (straight.Last().Value == Value.Ass)
                        {
                            combs.Add(new Combination(HandType.RoyalFlush, straight));
                        }
                    }
                }

            }

        }


        private static void shiftValueList()
        {
            //Bevor wir auf straßen überprüfen verschieben wir alle Karten nach links und
            //kopieren die Asse an die erste Stelle, falls es welche gibt.
            //Eine Straße kann nämlich auch von einem Ass aus starten.

            if (values.ContainsKey((int)Value.Ass))
            {
                List<Card> asse = values[(int)Value.Ass];

                for (int i = 13; i > 0; i--)
                {
                    if (values.ContainsKey(i - 1))
                    {
                        values[i] = values[i - 1];
                    }
                    else
                    {
                        if (values.ContainsKey(i))
                        {
                            values.Remove(i);
                        }
                    }
                }

                values[0] = asse;
            }
        }




        private static Card findKicker(List<Card> allCards, List<Card> exclude)
        {
            List<Card> otherCards = allCards.Except(exclude).ToList();
            otherCards.Sort();
            if (otherCards.Count > 0)
            {
                return otherCards.Last();
            }
            else
            {
                return null;
            }
        }
    }
}
