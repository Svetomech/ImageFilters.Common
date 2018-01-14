using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderBlurLight : ShaderModelFilter
    {
        public ShaderBlurLight(Bitmap image) : base(image) { }
        public int Radius = 1;

        protected override unsafe void Technique(int x, int y, ref byte[,,] result, RefPixelDelegate refPixel)
        {
            var (A, R, G, B) = RGB.GetBytesARGB();

            int radius = Radius;
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;

            byte* pixel = refPixel(x - radius, y, true);
            sumR += pixel[R];
            sumG += pixel[G];
            sumB += pixel[B];
            sumA += pixel[A];

            pixel = refPixel(x + radius, y, true);
            sumR += pixel[R];
            sumG += pixel[G];
            sumB += pixel[B];
            sumA += pixel[A];

            pixel = refPixel(x, y - radius, true);
            sumR += pixel[R];
            sumG += pixel[G];
            sumB += pixel[B];
            sumA += pixel[A];

            pixel = refPixel(x, y + radius, true);
            sumR += pixel[R];
            sumG += pixel[G];
            sumB += pixel[B];
            sumA += pixel[A];

            sumR /= 4;
            sumG /= 4;
            sumB /= 4;
            sumA /= 4;


            result[x, y, A] = (byte)sumA;
            result[x, y, R] = (byte)sumR;
            result[x, y, G] = (byte)sumG;
            result[x, y, B] = (byte)sumB;
        }
    }

}