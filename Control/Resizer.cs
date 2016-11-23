using System;
using System.Runtime.InteropServices;

namespace Bot
{
    static class Resizer
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        static public void resizeAndSwitch(IntPtr handle)
        {
            RECT rect;
            GetWindowRect(handle, out rect);
            int xSize = rect.right - rect.left;

            if (xSize != (1000))
            {
                MoveWindow(handle, 200, 50, 1000, 1000, true);
            }


            SwitchToThisWindow(handle);

        }
    }
}
