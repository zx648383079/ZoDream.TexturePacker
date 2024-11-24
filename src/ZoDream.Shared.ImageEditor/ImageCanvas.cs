using SkiaSharp;
using System;
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

        public void Mutate(IImageStyle style, Action<IImageCanvas> cb)
        {
            if (style.RotateDeg == 0)
            {
                cb(this);
                return;
            }
            var width = style.Width;
            var height = style.Height;
            if (style is IImageComputedStyle s)
            {
                width = s.ActualWidth;
                height = s.ActualHeight;
            }
            else
            {
                (width, height) =
                Drawing.SkiaExtension.ComputedRotate(style.Width, style.Height,
                style.RotateDeg);
            }
            var rotatedBitmap = new SKBitmap(width, height);
            using var surface = new SKCanvas(rotatedBitmap);
            surface.Translate(style.Width / 2, style.Height / 2);
            surface.RotateDegrees(style.RotateDeg);
            surface.Translate(-width / 2, -height / 2);
            cb.Invoke(new ImageCanvas(surface, styler)
            {
                X = -style.X, 
                Y = -style.Y
            });
            canvas.DrawBitmap(rotatedBitmap, style.X + X, style.Y + Y);
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

        private void Mutate(IImageStyle style, Action<SKCanvas, SKPoint> cb)
        {
            var point = new SKPoint(style.X + X, style.Y + Y);
            if (style.RotateDeg == 0)
            {
                cb(canvas, point);
                return;
            }
            var width = style.Width;
            var height = style.Height;
            if (style is IImageComputedStyle s)
            {
                width = s.ActualWidth;
                height = s.ActualHeight;
            } else
            {
                (width, height) =
                Drawing.SkiaExtension.ComputedRotate(style.Width, style.Height, 
                style.RotateDeg);
            }
            var rotatedBitmap = new SKBitmap(width, height);
            using var surface = new SKCanvas(rotatedBitmap);
            surface.Translate(style.Width / 2, style.Height / 2);
            surface.RotateDegrees(style.RotateDeg);
            surface.Translate(-width / 2, -height / 2);
            cb(surface, new SKPoint(0, 0));
            canvas.DrawBitmap(rotatedBitmap, point);
        }

        public void DrawBitmap(SKBitmap? source, IImageStyle style)
        {
            if (source is null)
            {
                return;
            }
            DrawBitmap(source, style.X, style.Y);
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
            DrawPicture(picture, style.X, style.Y);
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
            DrawSurface(surface, style.X, style.Y);
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
            DrawText(text, style.X, style.Y, textAlign, font, paint);
        }


    }
}
