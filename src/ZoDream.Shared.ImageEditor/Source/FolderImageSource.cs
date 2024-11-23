using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class FolderImageSource(IImageEditor editor) : BaseImageSource(editor)
    {
        public IImageLayer? Host {  get; set; }

        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            if (Host is null || !Host.IsVisible)
            {
                return; 
            }
            var c = canvas.Transform(X, Y);
            foreach (var item in Host.Children)
            {
                item.Paint(c);
            }
        }
    }
}
