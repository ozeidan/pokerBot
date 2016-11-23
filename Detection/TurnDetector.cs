using System;
using System.Drawing;

namespace Bot.Detection
{
    class TurnDetector
    {

        private Point relativeTurnPos = new Point(1654, 1334);
        private Point relativeTurn2Pos = new Point(1320, 1334);
        private IntPtr pokerHandle;

        public TurnDetector(IntPtr pokerHandle)
        {
            this.pokerHandle = pokerHandle;
        }

        public Boolean myTurn()
        {
            Resizer.resizeAndSwitch(pokerHandle);
            return checkPixel(relativeTurnPos);
        }

        public Boolean onlyCall()
        {
            Resizer.resizeAndSwitch(pokerHandle);
            return !checkPixel(relativeTurn2Pos);
        }


        private Boolean checkPixel(Point point)
        {
            Bitmap bitmapPixel = Screenshot.getRelativeScreenshot(point, new Size(1, 1), pokerHandle);

            if (bitmapPixel.GetPixel(0, 0).R > 50)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
