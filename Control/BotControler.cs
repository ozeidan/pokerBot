using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Bot.Data;

namespace Bot.Control
{
    internal class BotControler
    {
        private readonly IntPtr _pokerHandle;

        private Dictionary<string, Position> _posDict;


        public BotControler(IntPtr pokerHandle)
        {
            this._pokerHandle = pokerHandle;
            Init();
        }

        [DllImport("User32.Dll")]
        private static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private void Init()
        {
            _posDict = new Dictionary<string, Position>
            {
                {"Fold", new Position(570, 670)},
                {"call", new Position(740, 670)},
                {"raiseButton", new Position(900, 670)},
                {"raiseBox", new Position(726, 618)}
            };
        }

        public void Fold()
        {
            ClickAt("Fold");
        }

        public void Check()
        {
            ClickAt("call");
        }

        public void CallAll()
        {
            ClickAt("raiseButton");
        }

        public void Raise(int amount)
        {
            ClickAt("raiseBox");


            for (var i = 0; i < 4; i++)
            {
                SendKeys.Send("{BS}");
                Thread.Sleep(30);
                SendKeys.Send("{DEL}");
                Thread.Sleep(30);
            }

            var s = amount.ToString();

            foreach (var c in s)
            {
                SendKeys.SendWait(c.ToString());
                Thread.Sleep(100);
            }

            ClickAt("raiseButton");
        }


        private void ClickAt(string buttonName)
        {
            if (!_posDict.ContainsKey(buttonName))
                throw new ArgumentException();


            var pos = _posDict[buttonName];
            Rect rect;

            GetWindowRect(_pokerHandle, out rect);
            MoveAndClick(pos.X + rect.Left, pos.Y + rect.Top);
        }


        /// <summary>
        ///     Moves the mouse pointer to the specified location and simulates a Left mouse click
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>
        private static void MoveAndClick(int x, int y)
        {
            Point startPos;
            GetCursorPos(out startPos);

            var deltaX = x - startPos.X;
            var deltaY = y - startPos.Y;

            const int fraction = 10;

            var currentX = startPos.X;
            var currentY = startPos.Y;

            for (var i = 1; i <= fraction; i++)
            {
                currentX += deltaX / fraction;
                currentY += deltaY / fraction;

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

        private void CondSwitchWindow()
        {
            if (GetActiveWindow() == _pokerHandle) return;

            SwitchToThisWindow(_pokerHandle);
            Thread.Sleep(100);
        }
    }
}