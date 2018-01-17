using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderBlurLight : ShaderModelFilter
    {
        public ShaderBlurLight(Bitmap image) : base(image) { }
        public int Radius = 1;

        protected override unsafe void Technique(int x, int y)
        {
            byte* pixel = refPixel(x, y);

            int radius = Radius;
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;

            byte* _pixel = refPixel(x - radius, y, true);
            sumR += _pixel[R];
            sumG += _pixel[G];
            sumB += _pixel[B];
            sumA += _pixel[A];

            _pixel = refPixel(x + radius, y, true);
            sumR += _pixel[R];
            sumG += _pixel[G];
            sumB += _pixel[B];
            sumA += _pixel[A];

            _pixel = refPixel(x, y - radius, true);
            sumR += _pixel[R];
            sumG += _pixel[G];
            sumB += _pixel[B];
            sumA += _pixel[A];

            _pixel = refPixel(x, y + radius, true);
            sumR += _pixel[R];
            sumG += _pixel[G];
            sumB += _pixel[B];
            sumA += _pixel[A];

            sumR /= 4;
            sumG /= 4;
            sumB /= 4;
            sumA /= 4;


            buffer[x, y, A] = (byte)sumA;
            buffer[x, y, R] = (byte)sumR;
            buffer[x, y, G] = (byte)sumG;
            buffer[x, y, B] = (byte)sumB;
        }
    }

}