using System;
using System.Threading;
using Bot.Control;
using Bot.Detection;
using Bot.Odds;

namespace Bot
{
    internal class Player
    {
        private readonly BotControler _control;
        private readonly Eye _eye;
        private readonly int _id;


        public Player(IntPtr pokerHandle, int id)
        {
            _eye = new Eye(pokerHandle);
            _control = new BotControler(pokerHandle);
            _id = id;
        }

        public void Play()
        {
            while (true)
            {
                while (!_eye.MyTurn())
                    Thread.Sleep(2000);

                var playerCards = _eye.GetPlayerCards();
                var tableCards = _eye.GetTableCards();
                var hand = new MyHand(playerCards, tableCards);

                Console.WriteLine($"Player Hand: {hand.BestCombination}");


                var playerCount = _eye.GetPlayerCount();

                double minimumCallAmount = _eye.GetMinimumCall();
                double potAmount = _eye.GetPotAmount();


                if (!_eye.OnlyCall() && minimumCallAmount == 0)
                {
                    _control.Check();
                    Console.WriteLine("Checking");
                }
                else
                {
                    var potOdds = minimumCallAmount / (minimumCallAmount + potAmount) * 100;

                    double oddsOffset;

                    if (tableCards.Count == 0)
                        oddsOffset = (110 - minimumCallAmount) / 100;
                    else
                        oddsOffset = 0;


                    var winOdds = OddsCalculator.CalculateOdds(playerCards, tableCards, playerCount);


                    if (oddsOffset > 0)
                    {
                        winOdds *= 1 + oddsOffset;
                        Console.WriteLine($"Increasing Win Odds by {oddsOffset * 100:0,##}%");
                    }


                    Console.WriteLine($"Player {_id}: Pot Odds: {potOdds} | Odds of winning {winOdds}");

                    if (winOdds < potOdds)
                    {
                        Console.WriteLine("Folding");
                        _control.Fold();
                    }
                    else
                    {
                        Console.WriteLine("Calling");
                        if (_eye.OnlyCall())
                            _control.CallAll();
                        else
                            _control.Check();
                    }
                }

                Thread.Sleep(3000);
            }
        }
    }
}