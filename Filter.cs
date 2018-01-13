using System.Drawing;
using System.Drawing.Imaging;

namespace Svetomech.ImageFilters
{
    public abstract class Filter
    {
        public Filter(Bitmap image)
        {
            Image = image;
            ImageRectangle = new Rectangle(Point.Empty, image.Size);
        }

        public virtual Bitmap Image { get; protected set; }

        protected virtual Rectangle ImageRectangle { get; set; }

        public virtual Filter Apply()
        {
            var bitmap = Image.LockBits(ImageRectangle,
                ImageLockMode.ReadWrite, Image.PixelFormat);

            try { ApplyInternal(bitmap); }
            finally { Image.UnlockBits(bitmap); }

            return this;
        }

        protected abstract void ApplyInternal(BitmapData bitmap);
    }
}
