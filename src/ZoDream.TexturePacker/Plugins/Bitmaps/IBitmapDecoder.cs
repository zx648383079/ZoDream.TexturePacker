using SkiaSharp;
using System.IO;

namespace ZoDream.TexturePacker.Plugins.Bitmaps
{
    public interface IBitmapDecoder
    {
        public SKBitmap Decode(Stream input);
        public SKBitmap Decode(byte[] data);
        public SKBitmap Decode(string fileName);
        public byte[] Encode(SKBitmap data);
        public void Encode(SKBitmap data, Stream output);
        public void Encode(SKBitmap data, string fileName);
    }
}
