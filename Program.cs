using Bot.MyExceptions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Bot
{
    class MainClass
    {
        private static List<Player> playerList = new List<Player>();

        public static void Main(String[] args)
        {
            Thread myThread = new Thread(HandleGetter.getHandle);
            myThread.Start();

            IntPtr handle;

            while (true)
            {
                try
                {
                    handle = HandleGetter.Handle;

                    Console.WriteLine(String.Format("Found Poker Window. Starting Bot {0}.", playerList.Count + 1));

                    Player player = new Player(handle, playerList.Count + 1);
                    playerList.Add(player);

                    Thread thread = new Thread(player.play);
                    thread.Start();


                }
                catch (WindowNotFoundException)
                {

                }
                Thread.Sleep(250);
            }
        }
    }
}
