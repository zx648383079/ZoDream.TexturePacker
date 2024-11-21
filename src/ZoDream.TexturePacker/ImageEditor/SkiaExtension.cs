using SkiaSharp;
using System.Linq;
using ZoDream.Shared.Drawing;
using ZoDream.TexturePacker.Drawing;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.ImageEditor
{
    public static class SkiaExtension
    {


        public static SKBitmap PaintRotate(this IImageLayer layer, float angle)
        {
            var (rotatedWidth, rotatedHeight) = Shared.Drawing.SkiaExtension.ComputedRotate(layer.Width, layer.Height, angle);
            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);
            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees(angle);
                surface.Translate(-layer.Width / 2, -layer.Height / 2);
                layer.Paint(surface);
            }
            return rotatedBitmap;
        }


        public static SKPath CreatePath(SpriteUvLayer uv, int imageWidth, int imageHeight)
        {
            imageWidth -= 1;
            imageHeight -= 1;
            var pointItems = uv.VertexItems.Select(v => new SKPoint(v.X * imageWidth, v.Y * imageHeight)).ToArray();
            var path = new SKPath();
            for (var i = 0; i < pointItems.Length; i++)
            {
                var point = pointItems[i];
                if (i < 1)
                {
                    path.MoveTo(point);
                }
                else
                {
                    path.LineTo(point);
                }
            }
            path.Close();
            //for (var i = 0; i < pointItems.Length - 1; i++)
            //{

            //    for (var j = i + 2; j < pointItems.Length - 1; j++)
            //    {
            //        if (LineIsIntersecting(pointItems[i], pointItems[i + 1], pointItems[j], pointItems[j + 1]))
            //        {
            //            path.AddPoly([pointItems[i], pointItems[j], pointItems[i + 1], pointItems[j + 1], ]);
            //        }
            //    }
            //}
            return path;
        }

        public static SKBitmap? Clip(this SKBitmap source, SpriteLayer layer)
        {
            if (layer is SpriteUvLayer uv)
            {
                return source.Clip(uv);
            }
            var bitmap = new SKBitmap(layer.Width, layer.Height);
            using var canvas = new SKCanvas(bitmap);
            // canvas.Clear(SKColors.Transparent);
            canvas.DrawBitmap(source, SKRect.Create(layer.X, layer.Y, 
                layer.Width, layer.Height), 
                SKRect.Create(0, 0, layer.Width, layer.Height), new SKPaint()
                {
                    FilterQuality = SKFilterQuality.High
                });
            return bitmap;
        }

        public static SKBitmap? Clip(this SKBitmap source, SpriteUvLayer layer)
        {
            var path = CreatePath(layer, source.Width, source.Height);
            var rect = path.Bounds;
            if (rect.IsEmpty || rect.Width < 1 || rect.Height < 1)
            {
                return null;
            }
            layer.X = (int)rect.Left;
            layer.Y = (int)rect.Top;
            var bitmap = new SKBitmap((int)rect.Width, (int)rect.Height);
            using var canvas = new SKCanvas(bitmap);
            canvas.DrawBitmap(source, rect, 
                SKRect.Create(0, 0, bitmap.Width, bitmap.Height), new SKPaint()
                {
                    FilterQuality= SKFilterQuality.High
                });
            path.Offset(-rect.Left, -rect.Top);
            //canvas.DrawPath(path, new SKPaint()
            //{
            //    Color = SKColors.Red,
            //    StrokeWidth = 1,
            //    Style = SKPaintStyle.Stroke,
            //});
            canvas.ClipPath(path, SKClipOperation.Difference);
            canvas.Clear();
            return bitmap;
        }


        public static IImageLayer? TryParse(this IImageData source, IImageEditor editor)
        {
            if (source is IConvertLayer c)
            {
                return c.ToLayer(editor);
            }
            var bitmap = source.ToBitmap();
            if (bitmap is null)
            {
                return null;
            }
            return new BitmapImageLayer(bitmap, editor);
        }
    }
}
