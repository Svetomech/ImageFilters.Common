using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public sealed class SepiaFilter : Filter
    {
        public SepiaFilter(Bitmap image) : base(image) { }

        protected override unsafe void ApplyInternal(BitmapData bitmap)
        {
            byte* firstPixel = (byte*)bitmap.Scan0;
            byte* lastPixel = firstPixel + (bitmap.Height * bitmap.Stride);

            byte pixelSize = (byte)(bitmap.Stride / bitmap.Width);
            var (A, R, G, B) = RGB.GetBytesARGB(); // TODO: RGBA support

            for (byte* pixel = firstPixel; pixel < lastPixel; pixel += pixelSize)
            {
                byte oldR = pixel[R], oldG = pixel[G], oldB = pixel[B];

                float newR = (oldR * .393f) + (oldG * .769f) + (oldB * .189f);
                float newG = (oldR * .349f) + (oldG * .686f) + (oldB * .168f);
                float newB = (oldR * .272f) + (oldG * .534f) + (oldB * .131f);

                pixel[R] = newR > byte.MaxValue ? byte.MaxValue : (byte)newR;
                pixel[G] = newG > byte.MaxValue ? byte.MaxValue : (byte)newG;
                pixel[B] = newB > byte.MaxValue ? byte.MaxValue : (byte)newB;
            }
        }
    }
}
