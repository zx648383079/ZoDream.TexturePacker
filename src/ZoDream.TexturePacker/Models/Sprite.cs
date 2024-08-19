using System.Collections.Generic;

namespace ZoDream.TexturePacker.Models
{
    public class SpriteLayer
    {
        public string Name { get; set; } = string.Empty;

        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public int Rotate { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class SpriteLayerSection
    {
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public int Width { get; set; }
        public int Height { get; set; }

        public IList<SpriteLayer> Items { get; set; } = [];
    }
}
