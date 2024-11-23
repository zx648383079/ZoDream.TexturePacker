using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class TextureImageSource(IImageEditor editor) : BaseImageSource(editor)
    {
        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            
        }
    }
}
