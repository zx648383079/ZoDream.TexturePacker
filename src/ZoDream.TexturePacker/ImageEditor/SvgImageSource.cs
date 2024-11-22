using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using Svg.Skia;
using ZoDream.Shared.Drawing;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class SvgImageSource : BaseImageSource
    {

        public SvgImageSource(SKSvg svg, IImageEditor editor)
            : base(editor)
        {
            Source = svg;
            if (svg.Picture is null)
            {
                return;
            }
            var rect = svg.Picture!.CullRect;
            Width = (int)rect.Width;
            Height = (int)rect.Height;
        }
        public SKSvg Source { get; set; }

        public override BitmapSource? GetPreviewSource()
        {
            return Source.Picture?.CreateThumbnail(60).ToWriteableBitmap();
        }

        public override void Paint(IImageCanvas canvas)
        {
            canvas.DrawPicture(Source.Picture, X, Y);
        }
    }
}
