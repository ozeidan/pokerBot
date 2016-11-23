using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Detection
{
    class Screenshot
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        private static Bitmap getScreen(Point position, Size size)
        {
            var image = new Bitmap(size.Width, size.Height);
            var gfx = Graphics.FromImage(image);
            gfx.CopyFromScreen(position, new Point(0, 0), size, CopyPixelOperation.SourceCopy);

            return image;
        }

        public static Bitmap getRelativeScreenshot(Point position, Size size, IntPtr handle)
        {
            RECT rect;
            GetWindowRect(handle, out rect);
            rect.left += 7;

            rect.left *= 2;
            rect.top *= 2;

            position.X += rect.left;
            position.Y += rect.top;

            return getScreen(position, size);
        }
    }
}
