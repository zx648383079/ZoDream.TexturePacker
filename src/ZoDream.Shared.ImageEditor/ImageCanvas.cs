using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageCanvas(SKCanvas canvas, IImageStyler styler) : IImageCanvas
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IImageCanvas Transform(int x, int y)
        {
            if (x == 0 &&  y == 0)
            {
                return this;
            }
            return new ImageCanvas(canvas, styler) 
            {
                X = X + x,
                Y = Y + y,
            };
        }

        public IImageStyle Compute(IImageLayer layer)
        {
            return styler.Compute(layer);
        }

        public void DrawBitmap(SKBitmap? source, int x, int y)
        {
            if (source is null)
            {
                return;
            }
            canvas.DrawBitmap(source, x + X, y + Y);
        }

        public void DrawBitmap(SKBitmap? source, IImageStyle style)
        {
            if (source is null)
            {
                return;
            }
            //var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);
            //using (var surface = new SKCanvas(rotatedBitmap))
            //{
            //    surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
            //    surface.RotateDegrees(angle);
            //    surface.Translate(-layer.Width / 2, -layer.Height / 2);
            //    layer.Paint(new ImageCanvas(surface));
            //}
            canvas.DrawBitmap(source, style.X + X, style.Y + Y);
        }

        public void DrawPicture(SKPicture? picture, int x, int y)
        {
            if (picture is null)
            {
                return;
            }
            canvas.DrawPicture(picture, x + X, y + Y);
        }

        public void DrawPicture(SKPicture? picture, IImageStyle style)
        {
            if (picture is null)
            {
                return;
            }
            canvas.DrawPicture(picture, style.X + X, style.Y + Y);
        }

        public void DrawSurface(SKSurface? surface, int x, int y)
        {
            if (surface is null)
            {
                return;
            }
            canvas.DrawSurface(surface, x + X, y + Y);
        }

        public void DrawSurface(SKSurface? surface, IImageStyle style)
        {
            if (surface is null)
            {
                return;
            }
            canvas.DrawSurface(surface, style.X + X, style.Y + Y);
        }

        public void DrawText(string text, int x, int y, SKTextAlign textAlign, SKFont font, SKPaint paint)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            canvas.DrawText(text, x + X, y + Y, textAlign, font, paint);
        }

        public void DrawText(string text, IImageStyle style, SKTextAlign textAlign, SKFont font, SKPaint paint)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            canvas.DrawText(text, style.X + X, style.Y + Y, textAlign, font, paint);
        }


    }
}
