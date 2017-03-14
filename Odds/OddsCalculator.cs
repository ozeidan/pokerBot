using System.Collections.Generic;
using HoldemHand;

namespace Bot.Odds
{
    internal static class OddsCalculator
    {
        public static double CalculateOdds(List<Data.Card> playerCards,
            List<Data.Card> tableCards, int alivePlayers)
        {
            //Flop/turn/river calcs
            if (tableCards.Count <= 0) return HoleCardLookup.GetOdds(playerCards, alivePlayers);

            var cards = Hand.ParseHand(CardConverter.ConvertCards(playerCards));
            var table = Hand.ParseHand(CardConverter.ConvertCards(tableCards));

            const long durationMillis = 4500; //Has to be figured out

            var winOdds = MonteCarlo.WinOddsMonteCarlo(cards, table, alivePlayers, durationMillis);

            return winOdds;
        }
    }
}