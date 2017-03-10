using System.Collections.Generic;
using System.Linq;
using Bot.Data;

namespace Bot.Combinations
{
    public enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeofaKind,
        Straight,
        Flush,
        FullHouse,
        FourofaKind,
        StraightFlush,
        RoyalFlush,
        Error
    }

    internal static class CombDetector
    {
        private static SortedSet<Combination> _combinations;
        private static List<Data.Card> _cards;
        private static Dictionary<int, List<Data.Card>> _values;

        public static SortedSet<Combination> DetectCombinations(List<Data.Card> cards)
        {
            _combinations = new SortedSet<Combination>();
            CombDetector._cards = cards;

            //Höchste Karte wird zur High Card gemacht (FindHighCard)
            //Zweithöchste Karte ist der Kicker
            CleanUp(cards);

            cards.Sort();

            FindHighCard();

            InitValueList();

            FindDoublesTripels();

            FindMultiples();

            if (FindFlush(cards) != null)
                _combinations.Add(new Combination(HandType.Flush, FindFlush(cards)));

            ShiftValueList();

            return _combinations;
        }

        private static void CleanUp(List<Data.Card> cards)
        {
            var removeList = cards.Where(card => card.Value == Value.Error || card.Suit == Suit.Error).ToList();

            foreach (var card in removeList)
                cards.Remove(card);
        }


        private static void InitValueList()
        {
            //Karten werden in Listen nach Wert unterteilt, damit sie besser ausgewertet werden können

            _values = new Dictionary<int, List<Data.Card>>();
            foreach (var card in _cards)
            {
                var value = (int) card.Value;

                if (!_values.ContainsKey(value))
                {
                    var thiscard = new List<Data.Card> {card};
                    _values.Add(value, thiscard);
                }
                else
                {
                    _values[value].Add(card);
                }
            }
        }

        private static void FindHighCard()
        {
            var highCard = _cards.Last();
            var oneCard = new List<Data.Card> {highCard};

            _combinations.Add(_cards.Count > 1
                ? new Combination(HandType.HighCard, oneCard, _cards[_cards.Count - 2])
                : new Combination(HandType.HighCard, oneCard));
        }


        private static void FindDoublesTripels()
        {
            //Falls in value ein Eintrag eine Liste mit mehr als einem Eintrag ist gibt es ein Pair, TOK oder FOK
            foreach (var key in _values.Keys)
                if (_values[key].Count > 1)
                {
                    var cardList = _values[key];
                    var kicker = FindKicker(_cards, cardList);
                    var type = HandType.Error;

                    var amount = cardList.Count;

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

                    var pair = new Combination(type, cardList, kicker);
                    _combinations.Add(pair);
                }
        }

        private static List<Data.Card> FindFlush(List<Data.Card> cards)
        {
            var suitDict = new Dictionary<Suit, List<Data.Card>>();

            foreach (var card in cards)
            {
                if (!suitDict.ContainsKey(card.Suit))
                    suitDict[card.Suit] = new List<Data.Card>();

                suitDict[card.Suit].Add(card);
            }

            return (from key in suitDict.Keys where suitDict[key].Count >= 5 select suitDict[key]).FirstOrDefault();
        }

        private static void FindMultiples()
        {
            var doubleList = new List<Combination>();
            var tripleList = new List<Combination>();

            foreach (var pair in _combinations)
                if (pair.Type.Equals(HandType.OnePair))
                    doubleList.Add(pair);
                else if (pair.Type.Equals(HandType.ThreeofaKind))
                    tripleList.Add(pair);

            if (tripleList.Count == 2)
            {
                var triples = tripleList[0].Cards;
                triples.AddRange(tripleList[1].Cards);

                triples.Sort();

                _combinations.Add(new Combination(HandType.FullHouse, triples.Skip(1).ToList()));
            }


            if (doubleList.Count > 0 && tripleList.Count > 0)
            {
                var doubles = new List<Data.Card>();
                foreach (var pair in doubleList)
                    doubles.AddRange(pair.Cards);
                var triples = tripleList[0].Cards;

                triples.Sort();
                doubles.Sort();

                triples.AddRange(doubles.Take(2));

                _combinations.Add(new Combination(HandType.FullHouse, triples));
            }

            if (doubleList.Count > 1)
            {
                var cardList = new List<Data.Card>();
                foreach (var pair in doubleList)
                    cardList.AddRange(pair.Cards);

                cardList.Sort();
                var twoPairCards = cardList.GetRange(cardList.Count - 4, 4);
                var newKicker = FindKicker(_cards, twoPairCards);

                _combinations.Add(new Combination(HandType.TwoPair, twoPairCards, newKicker));
            }
        }


        private static void FindStraight()
        {
            FindStraight();

            var count = 0;

            for (var i = 0; i < 13; i++)
            {
                var amount = _values.ContainsKey(i) ? _values[i].Count : 0;

                if (amount > 0)
                    count++;
                else
                    count = 0;

                if (count < 5) continue;
                var straight = new List<Data.Card>();

                for (var j = i - 4; j < i + 1; j++)
                    straight.Add(_values[j][0]);

                _combinations.Add(new Combination(HandType.Straight, straight));


                if (FindFlush(straight) == null) continue;

                _combinations.Add(new Combination(HandType.StraightFlush, straight));

                if (straight.Last().Value == Value.Ass)
                    _combinations.Add(new Combination(HandType.RoyalFlush, straight));
            }
        }


        private static void ShiftValueList()
        {
            //Bevor wir auf straßen überprüfen verschieben wir alle Karten nach links und
            //kopieren die Asse an die erste Stelle, falls es welche gibt.
            //Eine Straße kann nämlich auch von einem Ass aus starten.

            if (!_values.ContainsKey((int) Value.Ass)) return;
            var asse = _values[(int) Value.Ass];

            for (var i = 13; i > 0; i--)
                if (_values.ContainsKey(i - 1))
                {
                    _values[i] = _values[i - 1];
                }
                else
                {
                    if (_values.ContainsKey(i))
                        _values.Remove(i);
                }

            _values[0] = asse;
        }


        private static Data.Card FindKicker(List<Data.Card> allCards, List<Data.Card> exclude)
        {
            var otherCards = allCards.Except(exclude).ToList();
            otherCards.Sort();
            return otherCards.Count > 0 ? otherCards.Last() : null;
        }
    }
}