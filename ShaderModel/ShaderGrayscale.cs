using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public class ShaderGrayscale : ShaderModelFilter
    {
        public ShaderGrayscale(Bitmap image) : base(image)
        {
            EnableBuffer = false;
            MultiThread = MultiThreadType.Thread4;
        }
        
        protected override unsafe void Technique(int x, int y)
        {
            byte* pixel = refPixel(x, y);
            pixel[R] = pixel[G] = pixel[B] = (byte)(math_mix(pixel[R], pixel[G], pixel[B]));
        }
    }

}