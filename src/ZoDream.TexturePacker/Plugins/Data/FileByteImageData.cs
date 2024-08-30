using SkiaSharp;

namespace ZoDream.TexturePacker.Plugins
{
    public class FileByteImageData(byte[] buffer) : BaseImageData
    {
        public override SKBitmap? TryParse()
        {
            return SKBitmap.Decode(buffer);
        }
    }
}
