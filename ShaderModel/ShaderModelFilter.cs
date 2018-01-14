using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace Svetomech.ImageFilters
{
    public class ShaderModelFilter : Filter
    {
        public ShaderModelFilter(Bitmap image) : base(image) { }

        public bool MultiThread = true;
        protected unsafe delegate byte* RefPixelDelegate(int x, int y, bool safeMode = true);

        protected override unsafe void ApplyInternal(BitmapData bitmap)
        {
            byte* firstPixel = (byte*)bitmap.Scan0;
            byte* lastPixel = firstPixel + (bitmap.Height * bitmap.Stride);

            byte pixelSize = (byte)(bitmap.Stride / bitmap.Width);
            var (A, R, G, B) = RGB.GetBytesARGB();

            img_width = bitmap.Width;
            img_height = bitmap.Height;

            byte[,,] result = new byte[bitmap.Width, bitmap.Height, 4];
            byte* RefPixel(int x, int y, bool safeMode = true)
            {
                if (safeMode)
                {
                    x = x < 0 ? 0 : x;
                    x = x >= bitmap.Width ? bitmap.Width - 1 : x;
                    y = y < 0 ? 0 : y;
                    y = y >= bitmap.Height ? bitmap.Height - 1 : y;
                }
                return (byte*)(x * pixelSize + bitmap.Stride * y + firstPixel);
            }
            void ShowResult()
            {
                for (int iy = 0; iy < bitmap.Height; iy++)
                    for (int ix = 0; ix < bitmap.Width; ix++)
                    {
                        byte* pixel = RefPixel(ix, iy, false);
                        pixel[A] = result[ix, iy, A];
                        pixel[R] = result[ix, iy, R];
                        pixel[G] = result[ix, iy, G];
                        pixel[B] = result[ix, iy, B];
                    }
            }

            if (MultiThread)
            {

                Thread[] THS = new Thread[3];
                THS[0] = new Thread(() =>
                {
                    for (int iy = 0; iy < bitmap.Height / 2; iy++)
                        for (int ix = 0; ix < bitmap.Width / 2; ix++)
                            Technique(ix, iy, ref result, RefPixel);
                });
                THS[1] = new Thread(() =>
                {
                    for (int iy = bitmap.Height / 2; iy < bitmap.Height; iy++)
                        for (int ix = 0; ix < bitmap.Width / 2; ix++)
                            Technique(ix, iy, ref result, RefPixel);
                });
                THS[2] = new Thread(() =>
                {
                    for (int iy = 0; iy < bitmap.Height / 2; iy++)
                        for (int ix = bitmap.Width / 2; ix < bitmap.Width; ix++)
                            Technique(ix, iy, ref result, RefPixel);
                });

                for (int i = 0; i < THS.Length; i++)
                {
                    THS[i].IsBackground = false;
                    THS[i].Start();
                }

                // Эта часть в текущем потоке
                for (int iy = bitmap.Height / 2; iy < bitmap.Height; iy++)
                    for (int ix = bitmap.Width / 2; ix < bitmap.Width; ix++)
                        Technique(ix, iy, ref result, RefPixel);

                // Ожидание завершения всех потоков
                while (true)
                {
                    bool next = true;
                    for (int i = 0; i < THS.Length; i++)
                        if (THS[i].IsAlive)
                        {
                            next = false;
                            break;
                        }
                    if (next) break;
                }

            }
            else
            {
                // Без мультипоточности
                for (int iy = 0; iy < bitmap.Height; iy++)
                    for (int ix = 0; ix < bitmap.Width; ix++)
                        Technique(ix, iy, ref result, RefPixel);
            }

            ShowResult();
        }

        protected virtual unsafe void Technique(int x, int y, ref byte[,,] result, RefPixelDelegate refPixel) { }

        protected int img_width = 0;
        protected int img_height = 0;              

        protected int math_dist(int X1, int Y1, int X2, int Y2)
        {
            return (int)System.Math.Sqrt(
                (X1 - X2) * (X1 - X2) +
                (Y1 - Y2) * (Y1 - Y2)
                );
        }
        protected int math_dist(int X1, int Y1, int Z1, int X2, int Y2, int Z2)
        {
            return (int)System.Math.Sqrt(
                (X1 - X2) * (X1 - X2) +
                (Y1 - Y2) * (Y1 - Y2) +
                (Z1 - Z2) * (Z1 - Z2)
                );
        }
        protected int math_dist(int X1, int Y1, int Z1, int A1, int X2, int Y2, int Z2, int A2)
        {
            return (int)System.Math.Sqrt(
                (X1 - X2) * (X1 - X2) +
                (Y1 - Y2) * (Y1 - Y2) +
                (Z1 - Z2) * (Z1 - Z2) +
                (A1 - A2) * (A1 - A2)
                );
        }
        protected int math_abs(int num) { return num < 0 ? -num : num; }
        protected int math_sqrt(int num) { return (int)System.Math.Sqrt(num); }
        protected int math_sqr(int num) { return num * num; }
        protected int math_max(int n1, int n2) { return n1 > n2 ? n1 : n2; }
        protected int math_min(int n1, int n2) { return n1 < n2 ? n1 : n2; }
        protected int math_mix(int n1, int n2) { return (n1 + n2) / 2; }
        protected int math_mix(int n1, int n2, int n3) { return (n1 + n2 + n3) / 3; }
        protected int math_mix(int n1, int n2, int n3, int n4) { return (n1 + n2 + n3 + n4) / 4; }
    }
}
