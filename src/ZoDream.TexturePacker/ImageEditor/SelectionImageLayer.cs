using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    /// <summary>
    /// 选择高亮
    /// </summary>
    public class SelectionImageLayer(IImageEditor editor): BaseImageLayer(editor), ICommandImageLayer
    {
        private SKSurface? _surface;

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            Invalidate();
        }

        public void Resize(IImageLayer layer)
        {
            X = layer.X;
            Y = layer.Y;
            Resize(layer.Width, layer.Height);
        }
        public void Invalidate()
        {
            _surface?.Dispose();
            _surface = null;
        }

        private void RenderSurface()
        {
            var info = new SKImageInfo(Editor.ActualWidthI, Editor.ActualHeightI);
            _surface = SKSurface.Create(info);
            var canvas = _surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            using var paint = new SKPaint()
            {
                Color = new SKColor(0, 0, 0, 150),
                Style = SKPaintStyle.Fill,
                StrokeWidth = 0,
            };
            if (X > 0)
            {
                canvas.DrawRect(0, 0, X, info.Height, paint);
            }
            var right = X + Width;
            if (right < info.Width)
            {
                canvas.DrawRect(right, 0, info.Width - right, info.Height, paint);
            }
            if (Y > 0)
            {
                canvas.DrawRect(X, 0, Width, Y, paint);
            }
            var bottom = Y + Height;
            if (bottom < info.Height) 
            {
                canvas.DrawRect(X, bottom, Width, info.Height - bottom, paint);
            }
        }

        public override void Paint(SKCanvas canvas)
        {
            if (_surface == null)
            {
                RenderSurface();
            }
            canvas.DrawSurface(_surface, 0, 0);
        }

        public override void Dispose()
        {
            _surface?.Dispose();
        }
    }
}
