using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class GrayscaleFilter : Filter
    {
        public GrayscaleFilter(Bitmap image) : base(image) { }

        protected override unsafe void ApplyInternal(BitmapData bitmap)
        {
            byte* firstPixel = (byte*)bitmap.Scan0;
            byte* lastPixel = firstPixel + (bitmap.Height * bitmap.Stride);

            byte pixelSize = (byte)(bitmap.Stride / bitmap.Width);
            var (A, R, G, B) = RGB.GetBytesARGB(); // TODO: RGBA support

            for (byte* pixel = firstPixel; pixel < lastPixel; pixel += pixelSize)
            {
                byte intensity = (byte)((pixel[R] + pixel[G] + pixel[B]) / 3);

                pixel[R] = pixel[G] = pixel[B] = intensity;
            }
        }
    }
}
