using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Bot.Control
{
    class BotControler
    {
        [DllImport("User32.Dll")]
        private static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        private  Dictionary<string, Position> posDict;
        private  IntPtr pokerHandle;



        public BotControler(IntPtr pokerHandle)
        {
            this.pokerHandle = pokerHandle;
            init();
        }

        private void init()
        {
            posDict = new Dictionary<string, Position>();
            posDict.Add("fold", new Position(570, 670));
            posDict.Add("call", new Position(740, 670));
            posDict.Add("raiseButton", new Position(900, 670));
            posDict.Add("raiseBox", new Position(726, 618));
        }

        public void fold()
        {
            clickAt("fold");
        }

        public void check()
        {
            clickAt("call");
        }

        public void callAll()
        {
            clickAt("raiseButton");
        }

        public void raise(int amount)
        {
            clickAt("raiseBox");


            for(int i = 0; i < 4; i++)
            {
                SendKeys.Send("{BS}");
                Thread.Sleep(30);
                SendKeys.Send("{DEL}");
                Thread.Sleep(30);

            }

            string s = amount.ToString();

            foreach(Char c in s)
            {
                SendKeys.SendWait(c.ToString());
                Thread.Sleep(100);
            }

            clickAt("raiseButton");
        }
            

        private void clickAt(string buttonName)
        {
            if (!posDict.ContainsKey(buttonName))
                throw new ArgumentException();

            
            Position pos = posDict[buttonName];
            RECT rect;

            GetWindowRect(pokerHandle, out rect);
            moveAndClick(pos.x + rect.left, pos.y + rect.top);
        }




        /// <summary>
        /// Moves the mouse pointer to the specified location and simulates a left mouse click
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>

        private  void moveAndClick(int x, int y)
        {
            POINT startPos;
            GetCursorPos(out startPos);

            int deltaX = x - startPos.x;
            int deltaY = y - startPos.y;

            int fraction = 10;

            int currentX = startPos.x;
            int currentY = startPos.y;

            for (int i = 1; i <= fraction; i++)
            {
                currentX += (int)deltaX / fraction;
                currentY += (int)deltaY / fraction;

                SetCursorPos(currentX, currentY);

                Thread.Sleep(1);
            }

            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);

            Thread.Sleep(10);

            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);

            Thread.Sleep(40);

            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);

            Thread.Sleep(10);

            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
        }
        private void condSwitchWindow()
        {
            if (GetActiveWindow() != pokerHandle)
            {
                SwitchToThisWindow(pokerHandle);
                Thread.Sleep(100);
            }
        }
    }
}
