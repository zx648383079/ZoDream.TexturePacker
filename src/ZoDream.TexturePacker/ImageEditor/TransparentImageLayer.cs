using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class TransparentImageLayer(int width, int height, int size = 10) : IImageLayer
    {

        private SKSurface? _surface;

        private void RenderSurface()
        {
            var info = new SKImageInfo(width, height);
            _surface = SKSurface.Create(info);
            var canvas = _surface.Canvas;
            canvas.Clear(SKColors.White);
            using var grayPaint = new SKPaint()
            {
                Color = SKColors.LightGray,
                Style = SKPaintStyle.Fill,
                StrokeWidth = 0,
            };
            var columnCount = width / size;
            var rowCount = height / size;
            for (var i = 0; i < columnCount; i++)
            {
                for (var j = 0; j < rowCount; j ++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        continue;
                    }
                    canvas.DrawRect(i * size, j * size, size, size, grayPaint);
                }
            }
        }

        public void Paint(SKCanvas canvas, SKImageInfo info)
        {
            if (_surface == null) 
            {
                RenderSurface();
            }
            canvas.DrawSurface(_surface, 0, 0);
        }

        public void Dispose()
        {
            _surface?.Dispose();
        }
    }
}
