using System;
using System.Drawing;
using Patagames.Ocr;
using System.Text.RegularExpressions;

namespace Bot.Detection
{
    class NumberDetector
    {
        private Point relativePotPos = new Point(560, 426);
        private Size potSize = new Size(842, 72);

        private Point relativeCallPos = new Point(1317, 1343);
        private Point relativeRaisePos = new Point(1660, 1343);
        private Size callSize = new Size(262, 47);

        private Point relativeMoneyPos = new Point(952, 1073);
        private Point relativeMoneyPosRight = new Point(850, 1073);
        private Point moneyCheckPoint = new Point(1080, 1080);
        private Size moneySize = new Size(175, 35);



        private IntPtr pokerHandle;

        private OcrApi api = OcrApi.Create();

        private TurnDetector turnDetect;


        public NumberDetector(IntPtr pokerHandle)
        {
            this.pokerHandle = pokerHandle;
            this.turnDetect = new TurnDetector(pokerHandle);
            api.Init(Patagames.Ocr.Enums.Languages.English);
        }

        public int getPot()
        {
            Resizer.resizeAndSwitch(pokerHandle);
            return getNumber(relativePotPos, potSize, cutPotBitmap);
        }


        public int minimalContribution()
        {
            Resizer.resizeAndSwitch(pokerHandle);
            if (turnDetect.myTurn())
            {
                if (turnDetect.onlyCall())
                {
                    return getMinRaise();
                }
                else
                {
                    return getCall();
                }
            }
            else
            {
                return 0;
            }
        }

        public int minimalRaise()
        {
            Resizer.resizeAndSwitch(pokerHandle);
            if (turnDetect.myTurn())
            {
                if (turnDetect.onlyCall())
                {
                    return 0;
                }
                else
                {
                    return getMinRaise();
                }
            }
            else
            {
                return 0;
            }

        }

        public int getMoney()
        {
            Bitmap bitmap = Screenshot.getRelativeScreenshot(moneyCheckPoint, new Size(1, 1), pokerHandle);
            Point pos;

            if (rgbSum(bitmap.GetPixel(0, 0)) > 150)
            {
                pos = relativeMoneyPosRight;
            }
            else
            {
                pos = relativeMoneyPos;
            }


            return getNumber(pos, moneySize, cutMoneyBitmap);
        }


        private int getCall()
        {
            return getNumber(relativeCallPos, callSize, cutCallBitmap);
        }

        private int getMinRaise()
        {
            return getNumber(relativeRaisePos, callSize, cutCallBitmap);
        }



        private int getNumber(Point pos, Size size, Func<Bitmap, Bitmap> cut)
        {

            Bitmap bitmap = Screenshot.getRelativeScreenshot(pos, size, pokerHandle);

            try
            {
                bitmap = cut(bitmap);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }

            string d = api.GetTextFromImage(bitmap);

            Regex regex = new Regex(@"(\d)+((\.)(\d)+)*");
            Match match = regex.Match(d);
            string matchString = match.Value;

            matchString = matchString.Replace(".", "");

            return int.Parse(matchString);
        }

        private Bitmap cutPotBitmap(Bitmap potBitmap)
        {
            int y = potBitmap.Height / 2;
            int xLeft = 0;

            while (potBitmap.GetPixel(xLeft, y).G > 70)
            {
                xLeft++;
            }

            int xRight = potBitmap.Width - 1;

            while (potBitmap.GetPixel(xRight, y).G > 70)
            {
                xRight--;
            }

            Rectangle crop = new Rectangle(xLeft, 0, xRight - xLeft, potBitmap.Height);

            return potBitmap.Clone(crop, potBitmap.PixelFormat);
        }

        private Bitmap cutCallBitmap(Bitmap callBitmap)
        {
            int y = callBitmap.Height / 2;
            int xLeft = 0;

            while (rgbSum(callBitmap.GetPixel(xLeft, y)) < 400)
            {
                xLeft++;
            }

            int xRight = callBitmap.Width - 1;

            while (rgbSum(callBitmap.GetPixel(xRight, y)) < 400)
            {
                xRight--;
            }

            Rectangle crop = new Rectangle(Math.Max((xLeft - 20), 0), 0, Math.Min((xRight - xLeft + 30), callBitmap.Width), callBitmap.Height);

            return callBitmap.Clone(crop, callBitmap.PixelFormat);

        }

        private Bitmap cutMoneyBitmap(Bitmap moneyBitmap)
        {
            int y = moneyBitmap.Height / 2;
            int xLeft = 0;

            while (moneyCutCheck(moneyBitmap.GetPixel(xLeft, y)))
            {
                xLeft++;
            }

            int xRight = moneyBitmap.Width - 1;

            while (moneyCutCheck(moneyBitmap.GetPixel(xRight, y)))
            {
                xRight--;
            }

            Rectangle crop = new Rectangle(Math.Max(0, xLeft - 20), 0, Math.Min(xRight - xLeft + 30, moneyBitmap.Width), moneyBitmap.Height);

            return moneyBitmap.Clone(crop, moneyBitmap.PixelFormat);

        }

        private int rgbSum(Color pixel)
        {
            return pixel.R + pixel.G + pixel.B;
        }

        private bool moneyCutCheck(Color pixel)
        {
            if (pixel.R > 100 && pixel.G > 100 && pixel.B > 100)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
