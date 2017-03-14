using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bot.Properties;

namespace Bot.Odds
{
    internal static class HoleCardLookup
    {
        private static readonly Dictionary<string, List<double>> Table = new Dictionary<string, List<double>>();
        private static bool _init;

        private static void LoadTable()
        {
            var regex = new Regex(@"(\d){1,2}\.(\d){2}");

            var data = Resources.HoleCardOdds;
            var words = data.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var line in words)
            {
                var pairName = line.Substring(0, 5);

                var matches = regex.Matches(line);

                var odds = (from Match match in matches select double.Parse(match.Value, CultureInfo.InvariantCulture)).ToList();

                Table.Add(pairName, odds);
            }
        }


        public static double GetOdds(List<Data.Card> playerCards, int playerAmount)
        {
            if (!_init)
            {
                LoadTable();
                _init = true;
            }

            playerAmount--; //Da an der Stelle 0 der Tabelle die Odds bei einem Gegner stehen usw.

            if (playerCards[0].Value < playerCards[1].Value)
                playerCards = new List<Data.Card> {playerCards[1], playerCards[0]};

            var entryName = ParseCards(playerCards);

            return Table[entryName][playerAmount];
        }


        private static string ParseCards(List<Data.Card> playerCards)
        {
            if (playerCards.Count != 2)
                throw new ArgumentException("Es gibt immer nur 2 Hole Cards!!");

            var cards = CardConverter.ConvertCards(playerCards);

            var secondSuit = playerCards[0].Suit == playerCards[1].Suit ? "s" : "h";

            return cards[0] + "s " + cards[3] + secondSuit;
        }
    }
}