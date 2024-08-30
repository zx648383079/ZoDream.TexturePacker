using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.TexturePacker.Plugins.Data
{
    public class StreamImageData(Stream stream): BaseImageData
    {
        public override SKBitmap? TryParse()
        {
            return SKBitmap.Decode(stream);
        }
    }
}
