using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageLayer: IDisposable
    {
        public void Paint(SKCanvas canvas, SKImageInfo info);
    }
}
