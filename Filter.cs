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

        public virtual Filter Crop(Rectangle area)
        {
            if (!ImageRectangle.IntersectsWith(area))
            {
                Image = null;
                return this;
            }

            area.Intersect(ImageRectangle);
            ImageRectangle = area;

            // TODO: Slow AF, needs low-level rewrite
            Image = Image.Clone(area, Image.PixelFormat);
            return this;
        }

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
