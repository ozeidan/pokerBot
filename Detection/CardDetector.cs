using System;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Collections.Generic;

namespace Bot.Detection
{
    class CardDetector
    {

        private int playerCardMargin = 120;
        private int tableCardMargin = 130;
        private Size cropSize = new Size(50, 98);
        private Point relativePlayerCardPos = new Point(860, 900);
        private Point relativeTableCardPos = new Point(661, 507);
        private Point relativeCheckCardPos = new Point(60, 4);


        IntPtr pokerHandle;



        public CardDetector(IntPtr handle)
        {
            this.pokerHandle = handle;
        }


        private static Bitmap firstCrop(Bitmap source)
        {
            Point position = new Point();
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    Color pixel = source.GetPixel(x, y);
                    if (pixel.R == 255 && pixel.G == 255 || pixel.B == 255)
                    {
                        position = new Point(x, y);
                        goto found;
                    }

                }
            }
            throw new MyExceptions.NoCardException();
            found:
            Size size = new Size(source.Width - position.X, source.Height - position.Y);
            Rectangle rect = new Rectangle(position, size);
            return source.Clone(rect, source.PixelFormat);
        }


        private static Bitmap secondCrop(Bitmap source)
        {
            Point position = new Point();
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);
                    if (pixel.R != 255 || pixel.G != 255 || pixel.B != 255)
                    {
                        position = new Point(x, y);
                        goto found;
                    }

                }
            }
            throw new MyExceptions.NoCardException();
            found:
            Rectangle rect = new Rectangle(position, new Size(10, 50));
            return source.Clone(rect, source.PixelFormat);
        }


        public List<Card> getCards(bool playerCards)
        {
            Resizer.resizeAndSwitch(pokerHandle);

            int cardAmount;
            int margin;
            Point pos;

            if (playerCards)
            {
                cardAmount = 2;
                margin = playerCardMargin;
                pos = relativePlayerCardPos;
            }
            else
            {
                cardAmount = getTableCardAmount();
                margin = tableCardMargin;
                pos = relativeTableCardPos;
            }

            List<Card> cards = new List<Card>();


            for (int i = 0; i < cardAmount; i++)
            {
                try
                {
                    var bitmap = Screenshot.getRelativeScreenshot(new Point(pos.X + (i * margin), pos.Y),
                                                                                  cropSize, pokerHandle);

                    bitmap = firstCrop(bitmap);
                    bitmap = secondCrop(bitmap);

                    string name = compareWithResources(bitmap);

                    cards.Add(new Card(name));
                }
                catch (MyExceptions.NoCardException)
                {
                    return new List<Card>();
                }
            }
            return cards;
        }

        private static String compareWithResources(Bitmap source)
        {
            ResourceSet resSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resSet)
            {
                if (entry.Value is Bitmap)
                {
                    Bitmap cmp = (Bitmap)entry.Value;

                    if (findBitmap(source, cmp))
                    {
                        return entry.Key.ToString();
                    }
                }
            }
            return "Not found";
        }


        public int getTableCardAmount()
        {
            Bitmap checkBitmap = Screenshot.getRelativeScreenshot(relativeTableCardPos, new Size(5 * tableCardMargin, cropSize.Height), pokerHandle);
            int checkX = relativeCheckCardPos.X;
            int checkCount = 0;
            Color pixel = checkBitmap.GetPixel(checkX, relativeCheckCardPos.Y);
            while (pixel.R == 255 && pixel.G == 255 && pixel.B == 255 && checkCount < 5)
            {
                checkCount++;

                if (checkCount < 5)
                {
                    checkX += tableCardMargin;
                    pixel = checkBitmap.GetPixel(checkX, relativeCheckCardPos.Y);
                }

            }
            return checkCount;
        }

        private static bool findBitmap(Bitmap bmpNeedle, Bitmap bmpHaystack)
        {
            if (bmpNeedle.Height != bmpHaystack.Height || bmpNeedle.Width != bmpHaystack.Width)
            {
                throw new Exception("Can't compare two bitmaps with different sizes!");
            }

            int pixelAmount = bmpHaystack.Width * bmpHaystack.Height;
            int pixelCount = 0;

            for (int x = 0; x < bmpNeedle.Width; x++)
            {
                for (int y = 0; y < bmpNeedle.Height; y++)
                {
                    Color cNeedle = bmpNeedle.GetPixel(x, y);
                    Color cHaystack = bmpHaystack.GetPixel(x, y);

                    if (cNeedle.R == cHaystack.R && cNeedle.G == cHaystack.G && cNeedle.B == cHaystack.B)
                    {
                        pixelCount++;
                    }
                }
            }
            if (pixelAmount == pixelCount)
                return true;
            else
                return false;
        }

    }
}
