using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderBorder : ShaderModelFilter
    {
        public ShaderBorder(Bitmap image) : base(image) { }

        protected override unsafe void Technique(int x, int y)
        {
            byte* pixel = refPixel(x, y);
            byte* _pixel = refPixel(x, y, false);
            buffer[x, y, A] = _pixel[A];

            byte distance(byte R1, byte G1, byte B1, byte R2, byte G2, byte B2) // оптимизировать через модули вместо квадратов и корней
            {
                return (byte)System.Math.Sqrt((R1 - R2) * (R1 - R2) + (G1 - G2) * (G1 - G2) + (B1 - B2) * (B1 - B2));
            }

            byte* npixel = refPixel(x + 1, y);
            byte dist = distance(_pixel[R], _pixel[G], _pixel[B], npixel[R], npixel[G], npixel[B]);
            dist = (byte)(dist > 32 ? 255 : 0);
            buffer[x, y, R] = buffer[x, y, G] = buffer[x, y, B] = dist;
        }
    }

}