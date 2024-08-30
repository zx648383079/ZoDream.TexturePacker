using SkiaSharp;

namespace ZoDream.TexturePacker.Plugins
{
    public class FileImageData(string fileName) : BaseImageData
    {
        public override SKBitmap? TryParse()
        {
            return SKBitmap.Decode(fileName);
        }
    }
}
