using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class ImageCanvas(SKCanvas canvas) : IImageCanvas
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IImageCanvas Transform(int x, int y)
        {
            if (x == 0 &&  y == 0)
            {
                return this;
            }
            return new ImageCanvas(canvas) 
            {
                X = X + x,
                Y = Y + y,
            };
        }

        public void DrawBitmap(SKBitmap? source, int x, int y)
        {
            if (source is null)
            {
                return;
            }
            canvas.DrawBitmap(source, x + X, y + Y);
        }

        public void DrawPicture(SKPicture? picture, int x, int y)
        {
            if (picture is null)
            {
                return;
            }
            canvas.DrawPicture(picture, x + X, y + Y);
        }

        public void DrawSurface(SKSurface? surface, int x, int y)
        {
            if (surface is null)
            {
                return;
            }
            canvas.DrawSurface(surface, x + X, y + Y);
        }

        public void DrawText(string text, int x, int y, SKTextAlign textAlign, SKFont font, SKPaint paint)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            canvas.DrawText(text, x + X, y + Y, textAlign, font, paint);
        }

        
    }
}
