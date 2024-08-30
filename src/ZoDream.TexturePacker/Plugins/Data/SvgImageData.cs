using SkiaSharp;
using Svg.Skia;
using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Plugins
{
    public class SvgImageData(string content) : IImageData
    {

        public SKBitmap? TryParse()
        {
            var svg = SKSvg.CreateFromSvg(content);
            return svg.Picture?.ToBitmap(SKColors.Transparent, 1, 1,
                SKColorType.Rgba8888, SKAlphaType.Premul, null);
        }

        public IImageLayer? TryParse(IImageEditor editor)
        {
            return new SvgImageLayer(SKSvg.CreateFromSvg(content), editor);
        }
    }
    public class SvgFileImageData(string fileName) : IImageData
    {
        public SKBitmap? TryParse()
        {
            var svg = new SKSvg();
            svg.Load(fileName);
            return svg.Picture?.ToBitmap(SKColors.Transparent, 1, 1,
                SKColorType.Rgba8888, SKAlphaType.Premul, null);
        }

        public IImageLayer? TryParse(IImageEditor editor)
        {
            var svg = new SKSvg();
            svg.Load(fileName);
            return new SvgImageLayer(svg, editor);
        }
    }

    
}
