using Svetomech.ImageFilters.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Svetomech.ImageFilters
{
    public abstract class ShaderModelFilter : Filter
    {
        public ShaderModelFilter(Bitmap image) : base(image) { }

        /// <summary>
        /// Во включённом состоянии использует 4 потока для обработки изображения.
        /// В среднем, увеличивает производительность на 25%
        /// Может оказаться не эффективным для малых изображений
        /// </summary>
        public MultiThreadType MultiThread { get; set; } = MultiThreadType.Thread4;
        /// <summary>
        /// Позволяет использовать вторичный буфер тех же размеров для сохранения результатов пикселей. 
        /// Этот буфер будет скопирован в изображение по окончании обработки. 
        /// При включении может замедлить обработку изображения не более чем в два раза. 
        /// Рекомендуется НЕ использовать буфер для простых операций (типо ЧБ или Сепия). 
        /// В выключенном состоянии буфер копироваться не будет, необходимо сохранять пиксели напрямую в массив изображения.
        /// </summary>
        public bool EnableBuffer { get; set; } = false;

        protected unsafe delegate byte* RefPixelDelegate(int x, int y, bool safeMode = true);
        public enum MultiThreadType
        {
            ThreadThis,
            Thread2,
            Thread4,
            Thread8,
            Thread16,
            Thread32,
            Thread64,
            Thread128,
            Thread256,
            Thread512,
            Thread1024,
        };

        protected override unsafe void ApplyInternal(BitmapData bitmap)
        {
            byte* firstPixel = (byte*)bitmap.Scan0;
            byte* lastPixel = firstPixel + (bitmap.Height * bitmap.Stride);

            byte pixelSize = (byte)(bitmap.Stride / bitmap.Width);
            (A, R, G, B) = RGB.GetBytesARGB();

            img_width = bitmap.Width;
            img_height = bitmap.Height;

            buffer = new byte[bitmap.Width, bitmap.Height, 4];
            refPixel = RefPixel;

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
            void ShowResult(Rectangle region)
            {
                for (int iy = region.Y; iy < region.Y + region.Height; iy++)
                    for (int ix = region.X; ix < region.X + region.Width; ix++)
                    {
                        byte* pixel = RefPixel(ix, iy, false);
                        pixel[A] = buffer[ix, iy, A];
                        pixel[R] = buffer[ix, iy, R];
                        pixel[G] = buffer[ix, iy, G];
                        pixel[B] = buffer[ix, iy, B];
                    }
            }
            void ApplyTechnique(Rectangle region)
            {
                for (int iy = region.Y; iy < region.Y + region.Height; iy++)
                {
                    for (int ix = region.X; ix < region.X + region.Width; ix++)
                    {
                        Technique(ix, iy);
                        data_tick++;
                    }
                }
                if (EnableBuffer) ShowResult(region);
            }
            void ThreadsSpawn(int xCount, int yCount)
            {
                var THSRegions = new List<Rectangle>();
                ParallelLoopResult THSResult;

                int regWidth = bitmap.Width / xCount;
                int regHeight = bitmap.Height / yCount;

                for (int iy = 0; iy < yCount; iy++)
                    for (int ix = 0; ix < xCount; ix++)
                        THSRegions.Add(new Rectangle(
                            ix * regWidth, 
                            iy * regHeight, 
                            regWidth, 
                            regHeight
                            ));

                Rectangle thisThreadRegion = THSRegions[0];
                THSRegions.RemoveAt(0);
                THSResult = Parallel.ForEach<Rectangle>(THSRegions, (Rectangle zone) => ApplyTechnique(zone));
                ApplyTechnique(thisThreadRegion);

                while (true)
                    if (THSResult.IsCompleted) break;
            }

            switch (MultiThread) {
                case MultiThreadType.ThreadThis:
                    {
                        ThreadsSpawn(1, 1);
                        break;
                    }
                case MultiThreadType.Thread2:
                    {
                        ThreadsSpawn(2, 1);
                        break;
                    }
                case MultiThreadType.Thread4:
                    {
                        ThreadsSpawn(2, 2);
                        break;
                    }
                case MultiThreadType.Thread8:
                    {
                        ThreadsSpawn(4, 2);
                        break;
                    }
                case MultiThreadType.Thread16:
                    {
                        ThreadsSpawn(4, 4);
                        break;
                    }
                case MultiThreadType.Thread32:
                    {
                        ThreadsSpawn(8, 4);
                        break;
                    }
                case MultiThreadType.Thread64:
                    {
                        ThreadsSpawn(8, 8);
                        break;
                    }
                case MultiThreadType.Thread128:
                    {
                        ThreadsSpawn(16, 8);
                        break;
                    }
                case MultiThreadType.Thread256:
                    {
                        ThreadsSpawn(16, 16);
                        break;
                    }
                case MultiThreadType.Thread512:
                    {
                        ThreadsSpawn(32, 16);
                        break;
                    }
                case MultiThreadType.Thread1024:
                    {
                        ThreadsSpawn(32, 32);
                        break;
                    }
            }
        }

        /// <summary>
        /// Метод обработки пикселей. Получает координаты пикселя который следует обработать.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected abstract void Technique(int x, int y);

        // Объекты доступные "шейдеру" // -----------------------------------

        protected RefPixelDelegate refPixel;

        protected byte[,,] buffer;

        protected int img_width = 0;
        protected int img_height = 0;

        protected int data_tick = 0;

        protected byte A, R, G, B;

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
