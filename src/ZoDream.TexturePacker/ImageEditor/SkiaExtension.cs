﻿using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.ImageEditor
{
    public static class SkiaExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="deg">360</param>
        public static void Rotate(this SKCanvas canvas, float deg)
        {
            canvas.RotateDegrees(deg);
        }

        public static void Flip(this SKCanvas canvas, bool isHorizontal = true)
        {
            if (isHorizontal)
            {
                canvas.Scale(1, -1);
            }
            else
            {
                canvas.Scale(-1, 1);
            }
        }

        /// <summary>
        /// 计算旋转后的外边框高度
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static (int, int) ComputedRotate(int width, int height, float angle)
        {
            var radians = Math.PI * angle / 180;
            var sine = Math.Abs(Math.Sin(radians));
            var cosine = Math.Abs(Math.Cos(radians));
            var originalWidth = width;
            var originalHeight = height;
            var rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            var rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);
            return (rotatedWidth, rotatedHeight);
        }

        public static SKBitmap Rotate(this SKBitmap bitmap, float angle)
        {
            var(rotatedWidth, rotatedHeight) = ComputedRotate(bitmap.Width, bitmap.Height, angle);
            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);
            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees(angle);
                surface.Translate(-bitmap.Width / 2, - bitmap.Height / 2);
                surface.DrawBitmap(bitmap, new SKPoint());
            }
            return rotatedBitmap;
        }

        public static SKBitmap PaintRotate(this IImageLayer layer, float angle)
        {
            var (rotatedWidth, rotatedHeight) = ComputedRotate(layer.Width, layer.Height, angle);
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

        public static SKEncodedImageFormat ConvertFormat(string extension)
        {
            var i = extension.LastIndexOf('.');
            if (i >= 0)
            {
                extension = extension[(i + 1)..];
            }
            return extension.ToLower() switch
            {
                "jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
                "ico" => SKEncodedImageFormat.Ico,
                "bmp" => SKEncodedImageFormat.Bmp,
                "webp" => SKEncodedImageFormat.Webp,
                "avif" => SKEncodedImageFormat.Avif,
                "gif" => SKEncodedImageFormat.Gif,
                "ktx" => SKEncodedImageFormat.Ktx,
                _ => SKEncodedImageFormat.Png
            };
        }

        public static void SaveAs(this SKBitmap bitmap, string fileName)
        {
            using var fs = File.OpenWrite(fileName);
            bitmap.Encode(fs, ConvertFormat(fileName), 100);
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

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static SKBitmap CreateThumbnail(this SKBitmap source, int size)
        {
            var bitmap = new SKBitmap(size, size);
            var scale = (float)size / Math.Max(source.Width, source.Height);
            var w = source.Width * scale;
            var h = source.Height * scale;
            using var canvas = new SKCanvas(bitmap);
            canvas.DrawBitmap(source, SKRect.Create((size - w) / 2, (size - h) / 2, w, h));
            return bitmap;
        }

        public static SKBitmap CreateThumbnail(this SKPicture source, int size)
        {
            var bitmap = new SKBitmap(size, size);
            var scale = size / Math.Max(source.CullRect.Width, source.CullRect.Height);
            var w = source.CullRect.Width * scale;
            var h = source.CullRect.Height * scale;
            using var canvas = new SKCanvas(bitmap);
            //canvas.DrawColor(SKColors.Transparent);
            canvas.Save();
            canvas.Scale(scale, scale);
            canvas.DrawPicture(source, (size - w) * scale / 2, (size - h) * scale / 2);
            canvas.Restore();
            return bitmap;
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

        private static bool LineIsIntersecting(SKPoint aBegin, SKPoint aEnd,
            SKPoint bBegin, SKPoint bEnd)
        {
            var aVector = aEnd - aBegin;
            var bVector = bEnd - bBegin;
            var cross = aVector.X * bVector.Y - aVector.Y * bVector.X;
            if (Math.Abs(cross) < 1e-8)
            {
                return false;
            }
            var diff = bBegin - aBegin;
            var t = (diff.X * bVector.Y - diff.Y * bVector.X) / cross;
            var u = (diff.X * aVector.Y - diff.Y * aVector.X) / cross;
            return 0 <= t && t <= 1 && 0 <= u && u <= 1;
        }
    }
}
