using SkiaSharp;
using Svg.Skia;
using System;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class SvgImageData(string content) : IImageData, IConvertLayer
    {

        public SKBitmap? ToBitmap()
        {
            var svg = SKSvg.CreateFromSvg(content);
            return svg.Picture?.ToBitmap(SKColors.Transparent, 1, 1,
                SKColorType.Rgba8888, SKAlphaType.Premul, null);
        }

        public SKImage? ToImage()
        {
            throw new NotImplementedException();
        }

        public IImageSource? ToLayer(IImageEditor editor)
        {
            return new SvgImageSource(SKSvg.CreateFromSvg(content), editor);
        }
    }
}
