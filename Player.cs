using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Bot.Detection;
using Bot.Control;
using Bot.Odds;

namespace Bot
{
    class Player
    {
        private Eye eye;
        private BotControler control;
        private int id;
        

        public Player(IntPtr pokerHandle, int id)
        {
            eye = new Eye(pokerHandle);
            control = new BotControler(pokerHandle);
            this.id = id;
        }

        public void play()
        {
            while (true)
            {
                while (!eye.myTurn())
                {
                    Thread.Sleep(2000);
                }

                var playerCards = eye.getPlayerCards();
                var tableCards = eye.getTableCards();
                var hand = new MyHand(playerCards, tableCards);
                Console.WriteLine(String.Format("Player Hand: {0}", hand.BestCombination.ToString()));
                

                int playerCount = eye.getPlayerCount();

                double minimumCallAmount = eye.getMinimumCall();
                double potAmount = eye.getPotAmount();


                if (!eye.onlyCall() && minimumCallAmount == 0)
                {
                    control.check();
                    Console.WriteLine("Checking");
                }
                else
                {

                    
                    double potOdds = minimumCallAmount / (minimumCallAmount + potAmount) * 100;

                    double testNumber;

                    if (tableCards.Count == 0)
                    {
                        testNumber = (110 - minimumCallAmount) / 100;
                    }
                    else
                    {
                        testNumber = 0;
                    }

                    

                    double winOdds = OddsCalculator.calculateOdds(playerCards, tableCards, playerCount);


                    if(testNumber > 0)
                    {
                        winOdds *= 1 + testNumber;
                        Console.WriteLine(String.Format("Increasing Win Odds by {0:0.00}%", (testNumber * 100)));
                    }
                    

                    Console.WriteLine(String.Format("Player {0}: Pot Odds: {1:0.00} | Odds of winning {2}", id, potOdds, winOdds));

                    if (winOdds < potOdds)
                    {
                        Console.WriteLine("Folding");
                        control.fold();
                    }
                    else
                    {
                        Console.WriteLine("Calling");
                        if (eye.onlyCall())
                        {
                            control.callAll();

                        }
                        else
                        {
                            control.check();
                        }
                    }
                }

                Thread.Sleep(3000);
            }


        }


    }
}
