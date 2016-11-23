using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bot.Odds
{
    static class HoleCardLookup
    {
        private static Dictionary<String, List<Double>> table = new Dictionary<String, List<Double>>();
        private static bool init = false;

        private static void loadTable()
        {
            Regex regex = new Regex(@"(\d){1,2}\.(\d){2}");

            string data = Properties.Resources.HoleCardOdds;
            List<string> words = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string line in words)
            {
                string pairName = line.Substring(0, 5);

                var matches = regex.Matches(line);

                List<double> odds = new List<double>();

                foreach (Match match in matches)
                {
                    odds.Add(Double.Parse(match.Value, CultureInfo.InvariantCulture));
                }

                table.Add(pairName, odds);
            }
        }


        public static double getOdds(List<Card> playerCards, int playerAmount)
        {
            if (!init)
            {
                loadTable();
                init = true;
            }

            playerAmount--; //Da an der Stelle 0 der Tabelle die Odds bei einem Gegner stehen usw.

            if(playerCards[0].Value < playerCards[1].Value)
            {
                playerCards = new List<Card>() { playerCards[1], playerCards[0] };
            }

            string entryName = parseCards(playerCards);

            return table[entryName][playerAmount];
        }


        private static string parseCards(List<Card> playerCards)
        {
            if (playerCards.Count != 2)
            {
                throw new ArgumentException("Es gibt immer nur 2 Hole Cards!!");
            }

            string cards = CardConverter.convertCards(playerCards);

            string secondSuit;

            if (playerCards[0].Suit == playerCards[1].Suit)
                secondSuit = "s";
            else
                secondSuit = "h";

            return cards[0] + "s " + cards[3] + secondSuit;
        }

    }
}
