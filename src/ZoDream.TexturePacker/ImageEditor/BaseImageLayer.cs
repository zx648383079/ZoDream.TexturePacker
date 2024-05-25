using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    public abstract class BaseImageLayer(Editor editor) : IImageLayer
    {

        protected Editor Editor { get; private set; } = editor;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public bool Visible { get; set; } = true;
        public abstract void Paint(SKCanvas canvas);

        public virtual void Dispose()
        {

        }
    }
}
