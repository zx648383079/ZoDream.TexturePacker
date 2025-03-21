﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.ImageEditor
{
    public static class EditorExtension
    {
        /// <summary>
        /// 修正角度
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static float ConvertAngle(float deg)
        {
            deg %= 360;
            if (deg < 0)
            {
                return deg + 360;
            }
            return deg;
        }

        public static (float, float) ComputedRotate(float width, float height, float angle)
        {
            var m = SKMatrix.CreateRotationDegrees(angle);
            var p = m.MapPoint(width, height);
            return (Math.Abs(p.X), Math.Abs(p.Y));
        }

        public static SKBitmap CreateThumbnail(this SKBitmap source, SKSizeI size)
        {
            return SkiaExtension.Mutate(size.Width, size.Height, canvas => {
                var scale = Math.Min((float)size.Width / source.Width, (float)size.Height / source.Height);
                var w = source.Width * scale;
                var h = source.Height * scale;
                canvas.DrawBitmap(source, SKRect.Create((size.Width - w) / 2, (size.Height - h) / 2, w, h));
            });
        }

        public static SKBitmap CreateThumbnail(this SKPicture source, SKSizeI size)
        {
            return SkiaExtension.Mutate(size.Width, size.Height, canvas => {
                var scale = Math.Min(size.Width / source.CullRect.Width, size.Height / source.CullRect.Height);
                var w = source.CullRect.Width * scale;
                var h = source.CullRect.Height * scale;
                //canvas.DrawColor(SKColors.Transparent);
                canvas.Save();
                canvas.Scale(scale, scale);
                canvas.DrawPicture(source, (size.Width - w) * scale / 2, (size.Height - h) * scale / 2);
                canvas.Restore();
            });
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
            var bitmap = new SKBitmap((int)layer.Width, (int)layer.Height);
            using var canvas = new SKCanvas(bitmap);
            // canvas.Clear(SKColors.Transparent);
            canvas.DrawBitmap(source, SKRect.Create(layer.X, layer.Y, 
                layer.Width, layer.Height), 
                SKRect.Create(0, 0, layer.Width, layer.Height), new SKPaint());
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
                SKRect.Create(0, 0, bitmap.Width, bitmap.Height), new SKPaint());
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


        public static IImageSource? TryParse(this IImageData source, IImageEditor editor)
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
            return new BitmapImageSource(bitmap, editor);
        }

        public static SKRect ToRect(this IImageBound bound) 
        {
            if (bound is IImageComputedStyle style)
            {
                return SKRect.Create(style.ActualLeft, style.ActualTop, style.ActualWidth, style.ActualHeight);
            }
            return SKRect.Create(bound.X, bound.Y, bound.Width, bound.Height);
        }

        public static SKMatrix ToMatrix(this IReadOnlyStyle style)
        {
            var scale = .1f;
            var x = 100;
            var y = -300;
            // return new SKMatrix(scale, 0, x, 0, scale, y, 0, 0, 1);
            return SKMatrix.Concat(
                new SKMatrix(style.ScaleX * scale, style.ShearX, style.X + x, 
                style.ShearY, style.ScaleY * scale, style.Y + y, 0, 0, 1),
                SKMatrix.CreateRotationDegrees(style.Rotate)
                );
        }

        /// <summary>
        /// 将UV转为图上坐标
        /// </summary>
        /// <param name="items"></param>
        /// <param name="bound"></param>
        /// <returns></returns>
        public static SKPoint[] ComputeVertex(IEnumerable<Vector2> items, IImageStyle bound)
        {
            var m = SKMatrix.CreateScale(bound.ScaleX * bound.Width, bound.ScaleY * bound.Height);
            m = m.PostConcat(SKMatrix.CreateRotationDegrees(bound.Rotate, bound.Width/ 2, bound.Height/ 2));
            // var p = m.MapPoint(bound.Width, bound.Height);
       
            return items.Select(i => m.MapPoint(i.X, i.Y)).ToArray();
        }
        /// <summary>
        /// 将UV转为图上坐标
        /// </summary>
        /// <param name="items"></param>
        /// <param name="matrix"></param>
        /// <param name="bound"></param>
        /// <returns></returns>
        public static SKPoint[] ComputeVertex(IEnumerable<Vector2> items, 
            SKMatrix matrix, 
            IImageSize bound)
        {
            return items.Select(i => matrix.MapPoint(i.X * bound.Width, i.Y * bound.Height)).ToArray();
        }
    }
}
