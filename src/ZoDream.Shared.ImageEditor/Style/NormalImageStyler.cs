using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    internal class NormalImageStyler : IImageStyler
    {
        public string Name => ImageStyleManager.DefaultName;

        public IImageStyle Compute(IImageLayer layer)
        {
            return new ImageComputedStyle(layer.Source, 0);
        }
    }
}
