using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class TextImageSource(string text, IImageEditor editor): BaseImageSource(editor)
    {
        private SKSurface? _surface;

        public string Text { get; set; } = text;

        public SKTypeface? FontFamily { get; set; }

        public int FontSize { get; set; } = 16;

        public SKColor Color { get; set; } = SKColors.Black;

        public SKTextAlign TextAlign { get; set; } = SKTextAlign.Left;

        private void RenderSurface()
        {
            using var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = Color;
            paint.IsStroke = false;
            using var font = new SKFont(FontFamily ?? SKTypeface.Default, FontSize);
            if (!font.ContainsGlyphs(Text))
            {
                // 乱码
                return;
            }
            var r = font.MeasureText(Text, out var bound, paint);
            Width = (int)bound.Width;
            Height = (int)bound.Height + 3;
            var info = new SKImageInfo(Width, Height);
            _surface = SKSurface.Create(info);
            var canvas = _surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            canvas.DrawText(Text, 0, (int)bound.Height, TextAlign, font, paint);
        }

        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            if (_surface == null)
            {
                RenderSurface();
            }
            canvas.DrawSurface(_surface, computedStyle);
        }

        public override void Dispose()
        {
            base.Dispose();
            _surface?.Dispose();
        }
    }
}
