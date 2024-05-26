using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    /// <summary>
    /// 改变尺寸框
    /// </summary>
    public class ResizeImageLayer(IImageEditor editor) : BaseImageLayer(editor), ICommandImageLayer
    {
        private int _dotSize = 10;
        private SKSurface? _surface;

        public float ActualLeft => X - _dotSize / 2;
        public float ActualTop => Y - _dotSize / 2;

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            _surface?.Dispose();
            _surface = null;
        }

        public void Resize(IImageLayer layer)
        {
            X = layer.X; 
            Y = layer.Y;
            Resize(layer.Width, layer.Height);
        }

        public void Invalidate()
        {
            
        }

        private void RenderSurface()
        {
            var info = new SKImageInfo(Width + _dotSize, Height + _dotSize);
            _surface = SKSurface.Create(info);
            var canvas = _surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            using var paint = new SKPaint()
            {
                Color = SKColors.Green,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
            };
            var xOffset = (float)Width / 2;
            var yOffset = (float)Height / 2;
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1)
                    {
                        continue;
                    }
                    canvas.DrawRect(i * xOffset, j * yOffset, _dotSize, _dotSize, paint);
                }
            }
            var dotOffset = (float)_dotSize / 2;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 3; j+=2)
                {
                    canvas.DrawLine(i * xOffset + _dotSize, j * yOffset + dotOffset, (i + 1) * xOffset + _dotSize, j * yOffset + dotOffset, paint);
                    canvas.DrawLine(j * xOffset + dotOffset, i * yOffset + _dotSize, j * xOffset + dotOffset, (i + 1) * yOffset + _dotSize, paint);
                }
            }
        }

        public override void Paint(SKCanvas canvas)
        {
            if (_surface == null)
            {
                RenderSurface();
            }
            canvas.DrawSurface(_surface, ActualLeft, ActualTop);
        }

        public override void Dispose()
        {
            _surface?.Dispose();
        }
    }
}
