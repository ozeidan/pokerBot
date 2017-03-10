using System;
using System.Drawing;
using Patagames.Ocr;
using System.Text.RegularExpressions;

namespace Bot.Detection
{
    internal class NumberDetector
    {
        private readonly OcrApi _api = OcrApi.Create();
        private readonly Size _callSize = new Size(262, 47);
        private readonly Point _moneyCheckPoint = new Point(1080, 1080);
        private readonly Size _moneySize = new Size(175, 35);


        private readonly IntPtr _pokerHandle;
        private readonly Size _potSize = new Size(842, 72);

        private readonly Point _relativeCallPos = new Point(1317, 1343);

        private readonly Point _relativeMoneyPos = new Point(952, 1073);
        private readonly Point _relativeMoneyPosRight = new Point(850, 1073);
        private readonly Point _relativePotPos = new Point(560, 426);
        private readonly Point _relativeRaisePos = new Point(1660, 1343);

        private readonly RoundDetector _turnDetect;


        public NumberDetector(IntPtr pokerHandle)
        {
            _pokerHandle = pokerHandle;
            _turnDetect = new RoundDetector(pokerHandle);
            _api.Init(Patagames.Ocr.Enums.Languages.English);
        }

        public int GetPot()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            return GetNumber(_relativePotPos, _potSize, CutPotBitmap);
        }


        public int MinimalContribution()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            if (!_turnDetect.MyTurn()) return 0;
            return _turnDetect.OnlyCall() ? GetMinRaise() : GetCall();
        }

        public int MinimalRaise()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            if (!_turnDetect.MyTurn()) return 0;
            return _turnDetect.OnlyCall() ? 0 : GetMinRaise();
        }

        public int GetMoney()
        {
            var bitmap = Screenshot.GetRelativeScreenshot(_moneyCheckPoint, new Size(1, 1), _pokerHandle);

            var pos = rgbSum(bitmap.GetPixel(0, 0)) > 150 ? _relativeMoneyPosRight : _relativeMoneyPos;


            return GetNumber(pos, _moneySize, CutMoneyBitmap);
        }


        private int GetCall()
        {
            return GetNumber(_relativeCallPos, _callSize, CutCallBitmap);
        }

        private int GetMinRaise()
        {
            return GetNumber(_relativeRaisePos, _callSize, CutCallBitmap);
        }


        private int GetNumber(Point pos, Size size, Func<Bitmap, Bitmap> cut)
        {
            var bitmap = Screenshot.GetRelativeScreenshot(pos, size, _pokerHandle);

            try
            {
                bitmap = cut(bitmap);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }

            string d = _api.GetTextFromImage(bitmap);

            var regex = new Regex(@"(\d)+((\.)(\d)+)*");
            var match = regex.Match(d);
            var matchString = match.Value;

            matchString = matchString.Replace(".", "");

            return int.Parse(matchString);
        }

        private static Bitmap CutPotBitmap(Bitmap potBitmap)
        {
            var y = potBitmap.Height / 2;
            var xLeft = 0;

            while (potBitmap.GetPixel(xLeft, y).G > 70)
                xLeft++;

            var xRight = potBitmap.Width - 1;

            while (potBitmap.GetPixel(xRight, y).G > 70)
                xRight--;

            var crop = new Rectangle(xLeft, 0, xRight - xLeft, potBitmap.Height);

            return potBitmap.Clone(crop, potBitmap.PixelFormat);
        }

        private Bitmap CutCallBitmap(Bitmap callBitmap)
        {
            var y = callBitmap.Height / 2;
            var xLeft = 0;

            while (rgbSum(callBitmap.GetPixel(xLeft, y)) < 400)
                xLeft++;

            var xRight = callBitmap.Width - 1;

            while (rgbSum(callBitmap.GetPixel(xRight, y)) < 400)
                xRight--;

            var crop = new Rectangle(Math.Max(xLeft - 20, 0), 0, Math.Min(xRight - xLeft + 30, callBitmap.Width),
                callBitmap.Height);

            return callBitmap.Clone(crop, callBitmap.PixelFormat);
        }

        private Bitmap CutMoneyBitmap(Bitmap moneyBitmap)
        {
            var y = moneyBitmap.Height / 2;
            var xLeft = 0;

            while (moneyCutCheck(moneyBitmap.GetPixel(xLeft, y)))
                xLeft++;

            var xRight = moneyBitmap.Width - 1;

            while (moneyCutCheck(moneyBitmap.GetPixel(xRight, y)))
                xRight--;

            var crop = new Rectangle(Math.Max(0, xLeft - 20), 0, Math.Min(xRight - xLeft + 30, moneyBitmap.Width),
                moneyBitmap.Height);

            return moneyBitmap.Clone(crop, moneyBitmap.PixelFormat);
        }

        private int rgbSum(Color pixel)
        {
            return pixel.R + pixel.G + pixel.B;
        }

        private bool moneyCutCheck(Color pixel)
        {
            if (pixel.R > 100 && pixel.G > 100 && pixel.B > 100)
                return false;
            return true;
        }
    }
}