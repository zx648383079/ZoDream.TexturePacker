using SkiaSharp;

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
