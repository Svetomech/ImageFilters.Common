using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderBlur : ShaderModelFilter
    {
        public ShaderBlur(Bitmap image) : base(image) { }
        public int Radius = 1;

        protected override unsafe void Technique(int x, int y, ref byte[,,] result, RefPixelDelegate refPixel)
        {
            var (A, R, G, B) = RGB.GetBytesARGB();

            int radius = Radius;
            int square = (radius * 2 + 1) * (radius * 2 + 1);
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;
            for (int iy = -radius; iy <= radius; iy++)
                for (int ix = -radius; ix <= radius; ix++)
                {
                    byte* pixel = refPixel(ix + x, iy + y, true);
                    sumR += pixel[R];
                    sumG += pixel[G];
                    sumB += pixel[B];
                    sumA += pixel[A];
                }
            sumR /= square;
            sumG /= square;
            sumB /= square;
            sumA /= square;


            result[x, y, A] = (byte)sumA;
            result[x, y, R] = (byte)sumR;
            result[x, y, G] = (byte)sumG;
            result[x, y, B] = (byte)sumB;
        }
    }

}