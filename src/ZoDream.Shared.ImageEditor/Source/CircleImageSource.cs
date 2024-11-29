using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class CircleImageSource(IImageEditor editor) : BaseImageSource(editor)
    {
        public SKColor FillColor { get; set; }
        public SKColor StrokeColor { get; set; }

        public float StrokeWidth { get; set; }
        public float XRadius { get; set; }
        public float YRadius { get; set; }



        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            using var paint = new SKPaint()
            {
                ColorF = FillColor,
                Color = StrokeColor,
                StrokeWidth = StrokeWidth,
                Style = SKPaintStyle.StrokeAndFill,
            };
            if (XRadius == YRadius)
            {
                canvas.DrawCircle(X, Y, XRadius, paint);
                return;
            }
            canvas.DrawOval(X, Y, XRadius, YRadius, paint);
        }
    }
}
