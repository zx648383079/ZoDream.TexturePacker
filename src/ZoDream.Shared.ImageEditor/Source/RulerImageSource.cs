using SkiaSharp;
using System.Collections.Generic;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class RulerImageSource(IImageEditor editor) : BaseImageSource(editor), ICommandImageSource
    {


        private readonly HashSet<int> _horizontalLines = [];
        private readonly HashSet<int> _verticalLines = [];
        private SKSurface? _surface;

        public void With(IImageLayer layer)
        {
        }

        public void Resize(float width, float height)
        {
            Invalidate();
        }

        public void Invalidate()
        {
            Width = Editor.ActualWidthI;
            Height = Editor.ActualHeightI;
            _surface?.Dispose();
            _surface = null;
        }

        public void Paint(IImageCanvas canvas)
        {
            if (_surface == null)
            {
                RenderSurface();
            }
            if (_surface == null)
            {
                return;
            }
            canvas.DrawSurface(_surface, 0, 0);
        }

        private void RenderSurface()
        {
            if (Width == 0 || Height == 0)
            {
                return;
            }
            var info = new SKImageInfo((int)Width, (int)Height);
            _surface = SKSurface.Create(info);
            var canvas = _surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            using var paint = new SKPaint()
            {
                Color = SKColors.Green,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
            };
            foreach (var item in _horizontalLines)
            {
                canvas.DrawLine(0, item, Width, item, paint);
            }
            foreach (var item in _verticalLines)
            {
                canvas.DrawLine(item, 0, item, Height, paint);
            }
        }

        public override void Dispose()
        {
            _surface?.Dispose();
        }
    }
}
