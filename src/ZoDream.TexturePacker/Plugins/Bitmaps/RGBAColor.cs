using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.TexturePacker.Plugins.Bitmaps
{
    public struct RGBAColor(byte red, byte green, byte blue, byte alpha)
    {
        public byte R = red, G = green, B = blue, A = alpha;
    }
}
