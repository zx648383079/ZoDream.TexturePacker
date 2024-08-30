using SkiaSharp;
using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Plugins
{
    public abstract class BaseImageData: IImageData
    {

        public abstract SKBitmap? TryParse();

        public IImageLayer? TryParse(IImageEditor editor)
        {
            var bitmap = TryParse();
            if (bitmap is null)
            {
                return null;
            }
            return new BitmapImageLayer(bitmap, editor);
        }
    }
}
