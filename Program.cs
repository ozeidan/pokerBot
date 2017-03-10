using System;
using System.Collections.Generic;
using System.Threading;

namespace Bot
{
    internal class MainClass
    {
        private static readonly List<Player> playerList = new List<Player>();

        public static void Main(string[] args)
        {
            // Start static function that searches for the windows we need and calls Release() on our semaphore when it finds one
            var sema = new Semaphore(0, 5);
            ThreadStart start = () => HandleGetter.FindHandles("PokerStars", "NHLE", ref sema);
            var myThread = new Thread(start);


            while (true)
            {
                // Only passes through this if a new handle is present
                sema.WaitOne();


                var handle = HandleGetter.Handle;
                Console.WriteLine($"Found Poker Window. Starting Bot {playerList.Count + 1}.");

                var player = new Player(handle, playerList.Count + 1);
                playerList.Add(player);

                var thread = new Thread(player.Play);
                thread.Start();
            }
        }
    }
}