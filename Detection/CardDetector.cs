using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Bot.Exceptions;
using Bot.Properties;

namespace Bot.Detection
{
    internal class CardDetector
    {
        private Size _cropSize = new Size(50, 98);

        private const int PlayerCardMargin = 120;


        private readonly IntPtr _pokerHandle;
        private Point _relativeCheckCardPos = new Point(60, 4);
        private readonly Point _relativePlayerCardPos = new Point(860, 900);
        private readonly Point _relativeTableCardPos = new Point(661, 507);
        private const int TableCardMargin = 130;


        public CardDetector(IntPtr handle)
        {
            _pokerHandle = handle;
        }


        private static Bitmap FirstCrop(Bitmap source)
        {
            Point position;
            for (var x = 0; x < source.Width; x++)
            for (var y = 0; y < source.Height; y++)
            {
                var pixel = source.GetPixel(x, y);
                if ((pixel.R != 255 || pixel.G != 255) && pixel.B != 255) continue;
                position = new Point(x, y);
                goto found;
            }
            throw new NoCardException();
            found:
            var size = new Size(source.Width - position.X, source.Height - position.Y);
            var rect = new Rectangle(position, size);
            return source.Clone(rect, source.PixelFormat);
        }


        private static Bitmap SecondCrop(Bitmap source)
        {
            Point position;
            for (var y = 0; y < source.Height; y++)
            for (var x = 0; x < source.Width; x++)
            {
                var pixel = source.GetPixel(x, y);
                if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255) continue;
                position = new Point(x, y);
                goto found;
            }
            throw new NoCardException();
            found:
            var rect = new Rectangle(position, new Size(10, 50));
            return source.Clone(rect, source.PixelFormat);
        }


        public List<Data.Card> GetCards(bool playerCards)
        {
            Resizer.ResizeAndSwitch(_pokerHandle);

            int cardAmount;
            int margin;
            Point pos;

            if (playerCards)
            {
                cardAmount = 2;
                margin = PlayerCardMargin;
                pos = _relativePlayerCardPos;
            }
            else
            {
                cardAmount = GetTableCardAmount();
                margin = TableCardMargin;
                pos = _relativeTableCardPos;
            }

            var cards = new List<Data.Card>();


            for (var i = 0; i < cardAmount; i++)
                try
                {
                    var bitmap = Screenshot.GetRelativeScreenshot(new Point(pos.X + i * margin, pos.Y),
                        _cropSize, _pokerHandle);

                    bitmap = FirstCrop(bitmap);
                    bitmap = SecondCrop(bitmap);

                    var name = CompareWithResources(bitmap);

                    cards.Add(new Data.Card(name));
                }
                catch (NoCardException)
                {
                    return new List<Data.Card>();
                }
            return cards;
        }

        private static string CompareWithResources(Bitmap source)
        {
            var resSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resSet)
            {
                var value = entry.Value as Bitmap;
                if (value == null) continue;
                var cmp = value;

                if (FindBitmap(source, cmp))
                    return entry.Key.ToString();
            }
            return "Not found";
        }


        public int GetTableCardAmount()
        {
            var checkBitmap = Screenshot.GetRelativeScreenshot(_relativeTableCardPos,
                new Size(5 * TableCardMargin, _cropSize.Height), _pokerHandle);
            var checkX = _relativeCheckCardPos.X;
            var checkCount = 0;
            var pixel = checkBitmap.GetPixel(checkX, _relativeCheckCardPos.Y);
            while (pixel.R == 255 && pixel.G == 255 && pixel.B == 255 && checkCount < 5)
            {
                checkCount++;

                if (checkCount < 5)
                {
                    checkX += TableCardMargin;
                    pixel = checkBitmap.GetPixel(checkX, _relativeCheckCardPos.Y);
                }
            }
            return checkCount;
        }

        private static bool FindBitmap(Bitmap bmpNeedle, Bitmap bmpHaystack)
        {
            if (bmpNeedle.Height != bmpHaystack.Height || bmpNeedle.Width != bmpHaystack.Width)
                throw new Exception("Can't compare two bitmaps with different sizes!");

            var pixelAmount = bmpHaystack.Width * bmpHaystack.Height;
            var pixelCount = 0;

            for (var x = 0; x < bmpNeedle.Width; x++)
            for (var y = 0; y < bmpNeedle.Height; y++)
            {
                var cNeedle = bmpNeedle.GetPixel(x, y);
                var cHaystack = bmpHaystack.GetPixel(x, y);

                if (cNeedle.R == cHaystack.R && cNeedle.G == cHaystack.G && cNeedle.B == cHaystack.B)
                    pixelCount++;
            }
            if (pixelAmount == pixelCount)
                return true;
            return false;
        }
    }
}