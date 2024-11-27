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
            if (style.Rotate == 0)
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
                Drawing.SkiaExtension.ComputedRotate((int)(style.Width * Math.Abs(style.ScaleX)), 
                (int)(style.Height * Math.Abs(style.ScaleY)),
                style.Rotate);
            }
            if (width == 0 || height == 0)
            {
                return;
            }
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var c = surface.Canvas;
            c.Translate(width / 2, height / 2);
            c.RotateDegrees(style.Rotate);
            c.Scale(style.ScaleX, style.ScaleY);
            c.Translate(-style.Width / 2, -style.Height / 2);
            cb.Invoke(new ImageCanvas(c, styler)
            {
                X = -style.X, 
                Y = -style.Y
            });
            DrawSurface(surface, style.X, style.Y);
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
            if (style.Rotate == 0)
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
                style.Rotate);
            }
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var c = surface.Canvas;
            c.Translate(style.Width / 2, style.Height / 2);
            c.RotateDegrees(style.Rotate);
            c.Translate(-width / 2, -height / 2);
            cb(c, new SKPoint(0, 0));
            canvas.DrawSurface(surface, point);
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
