using SkiaSharp;

namespace ZoDream.Shared.EditorInterface
{
    public interface ICanvasOption
    {
        public int Width { get; }

        public int Height { get; }

        public SKColor Foreground { get; }
        public SKColor Background { get; }

        public int StrokeWidth { get; }
    }
}
