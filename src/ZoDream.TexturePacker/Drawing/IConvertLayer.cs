using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Drawing
{
    public interface IConvertLayer
    {
        public IImageLayer? ToLayer(IImageEditor editor);
    }
}
