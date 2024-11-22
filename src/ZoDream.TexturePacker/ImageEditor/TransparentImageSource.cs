using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class TransparentImageSource : BaseImageSource, ICommandImageSource
    {
        public TransparentImageSource(IImageEditor editor): base(editor)
        {
            Invalidate();
        }

        public TransparentImageSource(int size, IImageEditor editor) : this(editor)
        {
            _gridSize = size;
        }

        private int _gridSize = 10;

        private SKSurface? _surface;

        public void Resize(int width, int height)
        {
            Invalidate();
        }

        public void Resize(IImageSource layer)
        {
        }
        public void Invalidate()
        {
            Width = Editor.ActualWidthI;
            Height = Editor.ActualHeightI;
            _surface?.Dispose();
            _surface = null;
        }

        private void RenderSurface()
        {
            if (Width == 0 || Height == 0)
            {
                return;
            }
            var info = new SKImageInfo(Width, Height);
            _surface = SKSurface.Create(info);
            var canvas = _surface.Canvas;
            canvas.Clear(SKColors.White);
            using var grayPaint = new SKPaint()
            {
                Color = SKColors.LightGray,
                Style = SKPaintStyle.Fill,
                StrokeWidth = 0,
            };
            var columnCount = Width / _gridSize + 1;
            var rowCount = Height / _gridSize + 1;
            for (var i = 0; i < columnCount; i++)
            {
                for (var j = 0; j < rowCount; j ++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        continue;
                    }
                    canvas.DrawRect(i * _gridSize, j * _gridSize, _gridSize, _gridSize, grayPaint);
                }
            }
        }

        public override void Paint(IImageCanvas canvas)
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

        public override void Dispose()
        {
            _surface?.Dispose();
        }
    }
}
