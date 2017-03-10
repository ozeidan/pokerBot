using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Bot.Data;
using Point = System.Drawing.Point;

namespace Bot.Detection
{
    internal class Screenshot
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        private static Bitmap GetScreen(Point position, Size size)
        {
            var image = new Bitmap(size.Width, size.Height);
            var gfx = Graphics.FromImage(image);
            gfx.CopyFromScreen(position, new Point(0, 0), size, CopyPixelOperation.SourceCopy);

            return image;
        }

        public static Bitmap GetRelativeScreenshot(Point position, Size size, IntPtr handle)
        {
            Rect rect;
            GetWindowRect(handle, out rect);
            rect.Left += 7;

            rect.Left *= 2;
            rect.Top *= 2;

            position.X += rect.Left;
            position.Y += rect.Top;

            return GetScreen(position, size);
        }
    }
}