using SkiaSharp;
using Svg.Skia;
using System;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class SvgFileImageData(string fileName) : IImageData, IConvertLayer
    {
        public SKBitmap? ToBitmap()
        {
            var svg = new SKSvg();
            svg.Load(fileName);
            return svg.Picture?.ToBitmap(SKColors.Transparent, 1, 1,
                SKColorType.Rgba8888, SKAlphaType.Premul, null);
        }

        public SKImage? ToImage()
        {
            throw new NotImplementedException();
        }

        public IImageSource? ToLayer(IImageEditor editor)
        {
            var svg = new SKSvg();
            svg.Load(fileName);
            return new SvgImageSource(svg, editor);
        }
    }
}
