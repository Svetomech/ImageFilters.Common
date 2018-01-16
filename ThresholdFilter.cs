using Svetomech.ImageFilters.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public sealed class ThresholdFilter : Filter
    {
        private byte x = 50;

        public ThresholdFilter(Bitmap image) : base(image) { }

        public byte X
        {
            get => x;

            set
            {
                if (value > 100)
                    throw new ArgumentOutOfRangeException(nameof(X));

                x = value;
            }
        }

        protected override unsafe void ApplyInternal(BitmapData bitmap)
        {
            byte* firstPixel = (byte*)bitmap.Scan0;
            byte* lastPixel = firstPixel + (bitmap.Height * bitmap.Stride);

            byte pixelSize = (byte)(bitmap.Stride / bitmap.Width);
            var (A, R, G, B) = RGB.GetBytesARGB(); // TODO: RGBA support

            for (byte* pixel = firstPixel; pixel < lastPixel; pixel += pixelSize)
            {
                byte intensity = (byte)((pixel[R] + pixel[G] + pixel[B]) / 3);

                if (intensity >= byte.MaxValue * x / 100)
                    pixel[R] = pixel[G] = pixel[B] = byte.MaxValue;
                else
                    pixel[R] = pixel[G] = pixel[B] = 0;
            }
        }
    }
}
