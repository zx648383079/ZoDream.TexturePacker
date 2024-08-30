using SkiaSharp;

namespace ZoDream.TexturePacker.Plugins
{
    public class BitmapImageData(SKBitmap bitmap): BaseImageData
    {
        public override SKBitmap? TryParse()
        {
            return bitmap.Copy();
        }
    }
}
