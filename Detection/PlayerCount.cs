using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bot.Detection
{
    internal class PlayerCount
    {
        private readonly IntPtr _pokerHandle;

        private readonly List<Point> _relativePlayerPoints = new List<Point>
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


        public PlayerCount(IntPtr pokerHandle)
        {
            this._pokerHandle = pokerHandle;
        }

        private bool CheckPlayerState(int player)
        {
            var playerPos = _relativePlayerPoints[player - 1];

            var checkPixel = Screenshot.GetRelativeScreenshot(playerPos, new Size(1, 1), _pokerHandle);

            return checkPixel.GetPixel(0, 0).R > 100;
        }

        private int playerCount()
        {
            var count = 0;

            Resizer.ResizeAndSwitch(_pokerHandle);

            for (var i = 1; i <= 8; i++)
                if (CheckPlayerState(i))
                    count++;

            return count;
        }
    }
}