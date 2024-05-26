﻿using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class BitmapImageLayer: BaseImageLayer
    {

        public BitmapImageLayer(string fileName, Editor editor) : this(SKBitmap.Decode(fileName), editor)
        {
            
        }

        public BitmapImageLayer(SKBitmap bitmap, Editor editor): base(editor)
        {
            Source = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        public SKBitmap Source { get; set; }

        public IList<IImageLayer> Split(IEnumerable<LayerItem> items)
        {
            using var paint = new SKPaint()
            {
                FilterQuality = SKFilterQuality.High
            };
            return items.Select(item => {
                var bitmap = new SKBitmap(item.Width, item.Height);
                using var canvas = new SKCanvas(bitmap);
                // canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(Source, SKRect.Create(item.X, item.Y, item.Width, item.Height), SKRect.Create(0, 0, item.Width, item.Height), paint);
                return new BitmapImageLayer(bitmap, Editor) 
                {
                    X = item.X,
                    Y = item.Y,
                };
            }).ToArray();
        }

        public override void Paint(SKCanvas canvas)
        {
            // 设置插值质量为高质量
            using var paint = new SKPaint();
            paint.FilterQuality = SKFilterQuality.High;
            canvas.DrawBitmap(Source, X, Y/*, paint*/);
        }

        public override void Dispose()
        {
            Source.Dispose();
        }
    }
}
