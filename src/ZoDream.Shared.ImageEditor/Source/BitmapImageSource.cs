using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.ImageEditor
{
    public class BitmapImageSource: BaseImageSource
    {

        public BitmapImageSource(string fileName, IImageEditor editor) : this(SKBitmap.Decode(fileName), editor)
        {
            
        }

        public BitmapImageSource(SKBitmap bitmap, IImageEditor editor): base(editor)
        {
            Source = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        public SKBitmap Source { get; set; }

        public IList<IImageSource> Split(IEnumerable<SpriteLayer> items)
        {
            using var paint = new SKPaint();
            return items.Select(item => {
                var bitmap = new SKBitmap((int)item.Width, (int)item.Height);
                using var canvas = new SKCanvas(bitmap);
                // canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(Source, SKRect.Create(item.X, item.Y, item.Width, item.Height), SKRect.Create(0, 0, item.Width, item.Height), paint);
                return new BitmapImageSource(bitmap, Editor) 
                {
                    X = item.X,
                    Y = item.Y,
                };
            }).ToArray();
        }

        public BitmapImageSource? Split(SpriteLayer item)
        {
            if (item.Y < 0)
            {
                item.Y += Source.Height - item.Height;
            }
            if (item.X < 0)
            {
                item.X += Source.Width - item.Width;
            }
            var bitmap = Source.Clip(item);
            if (bitmap == null)
            {
                return null;
            }
            return new BitmapImageSource(bitmap, Editor)
            {
                X = item.X,
                Y = item.Y,
                Rotate = item.Rotate
            };
        }

        public override SKBitmap? CreateThumbnail(SKSizeI size)
        {
            return Source.CreateThumbnail(size);
        }

        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            // 设置插值质量为高质量
            canvas.DrawBitmap(Source, computedStyle);
        }

        public override void Dispose()
        {
            Source.Dispose();
        }

        
    }
}
