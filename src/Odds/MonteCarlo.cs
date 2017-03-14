using System;
using System.Diagnostics;
using HoldemHand;

namespace Bot.Odds
{
    internal class MonteCarlo
    {
        public static double WinOddsMonteCarlo(ulong pocket, ulong board, int nopponents,
            long duration)
        {
            double win = 0.0, count = 0.0;


            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < duration)
            {
                var boardmask = Hand.RandomHand(board, pocket, 5);
                var playerHandVal = Hand.Evaluate(pocket | boardmask);

                var deadmask = boardmask | pocket;

                var greaterthan = true;
                var greaterthanequal = true;

                var split = 1;

                for (var i = 0; i < nopponents; i++)
                {
                    var oppmask = Hand.RandomHand(deadmask, 2);
                    var oppHandVal = Hand.Evaluate(oppmask | boardmask);

                    deadmask |= oppmask;

                    if (playerHandVal < oppHandVal)
                    {
                        greaterthan = greaterthanequal = false;
                        break;
                    }
                    if (playerHandVal != oppHandVal) continue;
                    greaterthan = false;
                    split++;
                }

                if (greaterthan)
                    win += 1.0;
                else if (greaterthanequal)
                    win += 1.0 / split;

                count += 1.0;
            }

            Console.WriteLine("Calculated {0} possibilities in {1:0.0} seconds", count, duration / 1000.0);

            return count == 0.0 ? 0.0 : win / count * 100;
        }
    }
}