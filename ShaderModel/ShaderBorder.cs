using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderBorder : ShaderModelFilter
    {
        public ShaderBorder(Bitmap image) : base(image) { }

        protected override unsafe void Technique(int x, int y, ref byte[,,] result, RefPixelDelegate refPixel)
        {
            var (A, R, G, B) = RGB.GetBytesARGB();
            byte* pixel = refPixel(x, y, false);
            result[x, y, A] = pixel[A];

            byte distance(byte R1, byte G1, byte B1, byte R2, byte G2, byte B2) // оптимизировать через модули вместо квадратов и корней
            {
                return (byte)System.Math.Sqrt((R1 - R2) * (R1 - R2) + (G1 - G2) * (G1 - G2) + (B1 - B2) * (B1 - B2));
            }

            byte* npixel = refPixel(x + 1, y);
            byte dist = distance(pixel[R], pixel[G], pixel[B], npixel[R], npixel[G], npixel[B]);
            dist = (byte)(dist > 32 ? 255 : 0);
            result[x, y, R] = result[x, y, G] = result[x, y, B] = dist;
        }
    }

}