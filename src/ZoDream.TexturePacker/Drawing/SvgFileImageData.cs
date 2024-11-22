using SkiaSharp;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Drawing;
using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Drawing
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
