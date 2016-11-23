using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bot.Detection
{
    class PlayerCount
    {
        List<Point> relativePlayerPoints = new List<Point>()
        {
            new Point(364, 868),
            new Point(134, 582),
            new Point(246, 300),
            new Point(638, 164),
            new Point(1218, 162),
            new Point(1610, 298),
            new Point(1720, 588),
            new Point(1490, 870)
        };

        private IntPtr pokerHandle;


        public PlayerCount(IntPtr pokerHandle)
        {
            this.pokerHandle = pokerHandle;
        }

        private bool checkPlayerState(int player)
        {
            Point playerPos = relativePlayerPoints[player - 1];

            Bitmap checkPixel = Screenshot.getRelativeScreenshot(playerPos, new Size(1, 1), pokerHandle);

            if (checkPixel.GetPixel(0, 0).R > 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int playerCount()
        {
            int count = 0;

            Resizer.resizeAndSwitch(pokerHandle);

            for (int i = 1; i <= 8; i++)
            {
                if (checkPlayerState(i))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
