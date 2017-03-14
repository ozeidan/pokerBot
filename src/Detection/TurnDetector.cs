using System;
using System.Drawing;

namespace Bot.Detection
{
    internal class RoundDetector
    {
        private readonly IntPtr _pokerHandle;
        private readonly Point _relativeTurn2Pos = new Point(1320, 1334);
        // ToDo: Make detection dynamic, not hardcoded
        private readonly Point _relativeTurnPos = new Point(1654, 1334);

        public RoundDetector(IntPtr pokerHandle)
        {
            _pokerHandle = pokerHandle;
        }

        public bool MyTurn()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            return CheckPixel(_relativeTurnPos);
        }

        public bool OnlyCall()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            return !CheckPixel(_relativeTurn2Pos);
        }


        private bool CheckPixel(Point point)
        {
            var bitmapPixel = Screenshot.GetRelativeScreenshot(point, new Size(1, 1), _pokerHandle);

            return bitmapPixel.GetPixel(0, 0).R > 50;
        }
    }
}