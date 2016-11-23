using System;
using System.Collections.Generic;
using HoldemHand;

namespace Bot.Odds
{
    static class OddsCalculator
    {
        public static double calculateOdds(List<Card> playerCards,
            List<Card> tableCards, int alivePlayers)
        {
            //Flop/turn/river calcs
            if (tableCards.Count > 0)
            {
                ulong cards = Hand.ParseHand(CardConverter.convertCards(playerCards));
                ulong table = Hand.ParseHand(CardConverter.convertCards(tableCards));

                long durationMillis = 4500; //Has to be figured out

                var winOdds = MonteCarlo.WinOddsMonteCarlo(cards, table, alivePlayers, durationMillis);

                return winOdds;
            }
            //Preflop calcs
            else
            {
                return HoleCardLookup.getOdds(playerCards, alivePlayers);
            }
        }
    }
}
