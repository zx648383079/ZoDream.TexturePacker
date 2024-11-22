using Microsoft.UI.Xaml.Media;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageCommander
    {
        public IImageLayerTree Source { get; }
        public IImageEditor Instance { set; }

        public IImageLayer Create(IImageSource source);
    }
}
