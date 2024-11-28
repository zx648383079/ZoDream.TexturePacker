using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class RectImageSource(IImageEditor editor) : BaseImageSource(editor)
    {
        public SKColor FillColor { get; set; }
        public SKColor StrokeColor { get; set; }

        public float StrokeWidth { get; set; }

        public float LeftRadius { get; set; }
        public float TopRadius { get; set; }
        public float RightRadius { get; set; }
        public float BottomRadius { get; set; }

        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            using var paint = new SKPaint()
            {
                ColorF = FillColor,
                Color = StrokeColor,
                StrokeWidth = StrokeWidth,
                Style = SKPaintStyle.StrokeAndFill,
            };
            if (LeftRadius == 0 && TopRadius == 0 && RightRadius == 0 && BottomRadius == 0) 
            {
                canvas.DrawRect(this.ToRect(), paint);
                return;
            }
            using var rect = new SKRoundRect();
            rect.SetNinePatch(this.ToRect(), LeftRadius, TopRadius, RightRadius, BottomRadius);
            canvas.DrawRect(rect, paint);
        }
    }
}
