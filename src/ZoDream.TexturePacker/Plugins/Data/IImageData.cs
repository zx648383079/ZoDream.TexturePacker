using SkiaSharp;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Plugins.Bitmaps;

namespace ZoDream.TexturePacker.Plugins
{
    public interface IImageData
    {
        public SKBitmap? TryParse();

        public IImageLayer? TryParse(IImageEditor editor);
    }
}
