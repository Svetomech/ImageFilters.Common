using System.Drawing;

namespace Svetomech.ImageFilters
{
    public class ShaderExample : ShaderModelFilter
    {
        public ShaderExample(Bitmap image) : base(image)
        {            
            // Использование доп. буффера равного по размеру буфферу изображения. 
            // По окончании обработки он будет скопирован в буфер изображения.
            // Замедляет работу на 30-45%
            EnableBuffer = true;
            
            // Изображение будет обрабатываться одновременно 4 потоками
            // Даёт прирост на 5-20%
            MultiThread = MultiThreadType.Thread4;
        }
        
        // Прирост скорости в зависимости от установленных флагов
        // BF - buffer, MT - multi-thread
        // [BF] [  ]  // 1.00
        // [BF] [MT]  // 0.80 +20%
        // [  ] [  ]  // 0.55 +45%
        // [  ] [MT]  // 0.50 +50%

        protected override unsafe void Technique(int x, int y)
        {            
            byte* pixel = refPixel(x, y, false);

            buffer[x, y, A] = pixel[A];
            buffer[x, y, R] = pixel[R];
            buffer[x, y, G] = pixel[G];
            buffer[x, y, B] = pixel[B];
        }
    }

}