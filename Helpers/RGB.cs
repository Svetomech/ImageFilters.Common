namespace Svetomech.ImageFilters.Helpers
{
    public static class RGB
    {
        public enum ARGB : byte
        {
            B,
            G,
            R,
            A
        }

        public enum RGBA : byte
        {
            A,
            B,
            G,
            R
        }

        public static (byte A, byte R, byte G, byte B) GetBytesARGB() =>
            ((byte)ARGB.A, (byte)ARGB.R, (byte)ARGB.G, (byte)ARGB.B);

        public static (byte R, byte G, byte B, byte A) GetBytesRGBA() =>
            ((byte)RGBA.R, (byte)RGBA.G, (byte)RGBA.B, (byte)RGBA.A);
    }
}
