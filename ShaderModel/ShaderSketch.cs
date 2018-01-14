using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System;

namespace Svetomech.ImageFilters
{
    public class ShaderSketch : ShaderModelFilter
    {
        public ShaderSketch(Bitmap image) : base(image) { }

        protected override unsafe void Technique(int x, int y, ref byte[,,] result, RefPixelDelegate refPixel)
        {
            var (A, R, G, B) = RGB.GetBytesARGB();
            byte* pixel = refPixel(x, y, false);
            result[x, y, A] = pixel[A];

            byte* Rpixel = refPixel(x + 1, y);
            byte Rdist = (byte)math_dist(pixel[R], pixel[G], pixel[B], Rpixel[R], Rpixel[G], Rpixel[B]);
            byte* Lpixel = refPixel(x - 1, y);
            byte Ldist = (byte)math_dist(pixel[R], pixel[G], pixel[B], Lpixel[R], Lpixel[G], Lpixel[B]);
            byte* Dpixel = refPixel(x, y + 1);
            byte Ddist = (byte)math_dist(pixel[R], pixel[G], pixel[B], Dpixel[R], Dpixel[G], Dpixel[B]);
            byte* Upixel = refPixel(x, y - 1);
            byte Udist = (byte)math_dist(pixel[R], pixel[G], pixel[B], Upixel[R], Upixel[G], Upixel[B]);

            result[x, y, R] = result[x, y, G] = result[x, y, B] = (byte)(255 - math_mix(Rdist, Ldist, Ddist, Udist));
        }
    }

}