using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderEmpty : ShaderModelFilter
    {
        public ShaderEmpty(Bitmap image) : base(image) { }

        protected override unsafe void Technique(int x, int y, ref byte[,,] result, RefPixelDelegate refPixel)
        {
            var (A, R, G, B) = RGB.GetBytesARGB();
            byte* pixel = refPixel(x, y, false);

            result[x, y, A] = pixel[A];
            result[x, y, R] = pixel[R];
            result[x, y, G] = pixel[G];
            result[x, y, B] = pixel[B];
        }
    }

}