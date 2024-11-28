using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class PathImageSource(SKPath path, IImageEditor editor) : BaseImageSource(editor)
    {
        public SKColor FillColor { get; set; }
        public SKColor StrokeColor { get; set; }

        public float StrokeWidth { get; set; }

        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            using var paint = new SKPaint()
            {
                ColorF = FillColor,
                Color = StrokeColor,
                StrokeWidth = StrokeWidth,
                Style = SKPaintStyle.StrokeAndFill,
            };
            canvas.DrawPath(path, paint);
        }
    }
}
