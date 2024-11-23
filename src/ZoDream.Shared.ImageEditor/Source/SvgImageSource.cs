using SkiaSharp;
using Svg.Skia;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
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

        public override SKBitmap? CreateThumbnail(SKSizeI size)
        {
            return Source.Picture?.CreateThumbnail(size);
        }

        public override void Paint(IImageCanvas canvas, IImageStyle computedStyle)
        {
            canvas.DrawPicture(Source.Picture, computedStyle);
        }
    }
}
