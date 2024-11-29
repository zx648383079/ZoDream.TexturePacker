using SkiaSharp;
using System;
using System.Diagnostics;
using System.Linq;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageCanvas(SKCanvas canvas, IImageStyler styler) : IImageCanvas
    {
        public float X { get; set; }
        public float Y { get; set; }

        public IImageCanvas Transform(float x, float y)
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
                EditorExtension.ComputedRotate((int)(style.Width * Math.Abs(style.ScaleX)), 
                (int)(style.Height * Math.Abs(style.ScaleY)),
                style.Rotate);
            }
            if (width == 0 || height == 0)
            {
                return;
            }
            var info = new SKImageInfo((int)width, (int)height);
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

        public void DrawBitmap(SKBitmap? source, float x, float y)
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
                EditorExtension.ComputedRotate(style.Width, style.Height, 
                style.Rotate);
            }
            var info = new SKImageInfo((int)width, (int)height);
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
            if (style is IImageComputedVertexStyle u)
            {
                canvas.DrawBitmap(source, new SKPoint(style.X, style.Y));
                DrawTexture(source, u.SourceItems, u.PointItems);
                return;
            }
            if (style is IImageVertexStyle v)
            {
                DrawTexture(source, EditorExtension.ComputeVertex(v.VertexItems, style), v.PointItems);
                return;
            }
            DrawBitmap(source, style.X, style.Y);
        }

        public void DrawPicture(SKPicture? picture, float x, float y)
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

        public void DrawSurface(SKSurface? surface, float x, float y)
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

        public void DrawText(string text, float x, float y, SKTextAlign textAlign, SKFont font, SKPaint paint)
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

        public void DrawPath(SKPath path, SKPaint paint)
        {
            canvas.DrawPath(path, paint);
        }

        /// <summary>
        /// 绘制纹理
        /// </summary>
        /// <param name="source">纹理图片</param>
        /// <param name="sourceVertices">纹理上的顶点</param>
        /// <param name="vertices">顶点对于的位置</param>
        public void DrawTexture(SKBitmap source, 
            SKPoint[] sourceVertices, SKPoint[] vertices)
        {
            canvas.DrawPoints(SKPointMode.Polygon, sourceVertices, new SKPaint()
            {
                Color = SKColors.Red,
                IsStroke = true,
            });
            return;
            using var paint = new SKPaint()
            {
                IsAntialias = true,
                Shader = SKShader.CreateBitmap(source, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp)
            };
            canvas.DrawVertices(SKVertexMode.Triangles, 
                vertices, 
                sourceVertices, null, paint);
        }

        public void DrawRect(SKRect rect, SKPaint paint)
        {
            canvas.DrawRect(rect, paint);
        }

        public void DrawRect(SKRoundRect rect, SKPaint paint)
        {
            canvas.DrawRoundRect(rect, paint);
        }

        public void DrawCircle(float x, float y, float radius, SKPaint paint)
        {
            canvas.DrawCircle(x, y, radius, paint);
        }

        public void DrawOval(float x, float y, float xRadius, float yRadius, SKPaint paint)
        {
            canvas.DrawOval(x, y, xRadius, yRadius, paint);
        }
    }
}
