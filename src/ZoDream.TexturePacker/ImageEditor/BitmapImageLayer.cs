using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class BitmapImageLayer(SKBitmap bitmap): IImageLayer
    {

        public BitmapImageLayer(string fileName): this(SKBitmap.Decode(fileName))
        {
            
        }
        public int X { get; set; }

        public int Y { get; set; }

        public SKBitmap Source { get; set; } = bitmap;

        public void Paint(SKCanvas canvas, SKImageInfo info)
        {
            canvas.DrawBitmap(Source, X, Y);
        }

        public void Dispose()
        {
            Source.Dispose();
        }
    }
}
