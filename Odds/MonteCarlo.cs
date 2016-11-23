using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoldemHand;
using System.Diagnostics;

namespace Bot.Odds
{
    class MonteCarlo
    {
        public static double WinOddsMonteCarlo(ulong pocket, ulong board, int nopponents,
           long duration)
        {
            double win = 0.0, count = 0.0;


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < duration)
            {
                ulong boardmask = Hand.RandomHand(board, pocket, 5);
                uint playerHandVal = Hand.Evaluate(pocket | boardmask);

                ulong deadmask = boardmask | pocket;

                bool greaterthan = true;
                bool greaterthanequal = true;

                int split = 1;

                for (int i = 0; i < nopponents; i++)
                {
                    ulong oppmask = Hand.RandomHand(deadmask, 2);
                    uint oppHandVal = Hand.Evaluate(oppmask | boardmask);

                    deadmask |= oppmask;

                    if (playerHandVal < oppHandVal)
                    {
                        greaterthan = greaterthanequal = false;
                        break;
                    }
                    else if (playerHandVal == oppHandVal)
                    {
                        greaterthan = false;
                        split++;
                    }
                }

                if (greaterthan)
                    win += 1.0;
                else if (greaterthanequal)
                    win += 1.0 / split;

                count += 1.0;
            }

            Console.WriteLine(String.Format("Calculated {0} possibilities in {1:0.0} seconds", count, duration / 1000.0));

            return (count == 0.0 ? 0.0 : win / count * 100);
        }
    }
}
