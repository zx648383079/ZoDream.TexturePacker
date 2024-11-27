using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    /// <summary>
    /// 选择高亮
    /// </summary>
    public class SelectionImageSource(IImageEditor editor): BaseImageSource(editor), ICommandImageSource
    {
        private SKSurface? _surface;
        private IImageLayer? _target;

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            Invalidate();
        }

        public void With(IImageLayer layer)
        {
            _target = layer;
            SyncSize();
            Invalidate();
        }

        public void Invalidate()
        {
            _surface?.Dispose();
            _surface = null;
        }

        private void SyncSize()
        {
            if (_target is null)
            {
                return;
            }
            var style = Editor.ComputedStyler.Compute(_target);
            if (style is IImageComputedStyle c)
            {
                X = c.ActualLeft;
                Y = c.ActualTop;
                Width = c.ActualWidth;
                Height = c.ActualHeight;
                return;
            }
            X = style.X;
            Y = style.Y;
            Width = style.Width;
            Height = style.Height;
        }

        private void RenderSurface()
        {
            SyncSize();
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

        public void Paint(IImageCanvas canvas)
        {
            if (_surface == null)
            {
                RenderSurface();
            }
            canvas.DrawSurface(_surface, 0, 0);
        }

        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            Paint(canvas);
        }

        public override void Dispose()
        {
            _surface?.Dispose();
        }
    }
}
