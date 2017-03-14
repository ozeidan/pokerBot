using System;
using System.Runtime.InteropServices;
using Bot.Data;

namespace Bot
{
    internal static class Resizer
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public static void ResizeAndSwitch(IntPtr handle)
        {
            Rect rect;
            GetWindowRect(handle, out rect);
            var xSize = rect.Right - rect.Left;

            if (xSize != 1000)
                MoveWindow(handle, 200, 50, 1000, 1000, true);


            SwitchToThisWindow(handle);
        }
    }
}