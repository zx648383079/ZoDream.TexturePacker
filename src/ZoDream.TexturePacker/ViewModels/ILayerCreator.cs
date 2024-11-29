using ZoDream.Shared.EditorInterface;

namespace ZoDream.TexturePacker.ViewModels
{
    public interface ILayerCreator
    {

        public bool TryCreate(IImageEditor editor);
    }
}
