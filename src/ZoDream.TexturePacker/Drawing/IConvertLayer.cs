using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Drawing
{
    public interface IConvertLayer
    {
        public IImageSource? ToLayer(IImageEditor editor);
    }
}
