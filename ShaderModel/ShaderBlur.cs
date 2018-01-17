using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderBlur : ShaderModelFilter
    {
        public ShaderBlur(Bitmap image) : base(image)
        {
            MultiThread = MultiThreadType.Thread128;
            EnableBuffer = true;
        }
        public int Radius = 5;

        protected override unsafe void Technique(int x, int y)
        {            
            byte* pixel = refPixel(x, y);

            int radius = Radius;
            int square = (radius * 2 + 1) * (radius * 2 + 1);
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;
            for (int iy = -radius; iy <= radius; iy++)
                for (int ix = -radius; ix <= radius; ix++)
                {
                    byte* _pixel = refPixel(ix + x, iy + y, true);
                    sumR += _pixel[R];
                    sumG += _pixel[G];
                    sumB += _pixel[B];
                    sumA += _pixel[A];                    
                }
            sumR /= square;
            sumG /= square;
            sumB /= square;
            sumA /= square;


            buffer[x, y, A] = (byte)sumA;
            buffer[x, y, R] = (byte)sumR;
            buffer[x, y, G] = (byte)sumG;
            buffer[x, y, B] = (byte)sumB;
        }
    }

}