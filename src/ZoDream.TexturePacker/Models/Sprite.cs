using System.Collections.Generic;
using System.Numerics;
using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Models
{
    public class SpriteLayer: IImageBound
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

    public class SpriteUvLayer: SpriteLayer
    {
        public IList<Vector2> VertexItems { get; set; } = [];
    }

    public class SpriteLayerSection: IImageSize
    {
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public int Width { get; set; }
        public int Height { get; set; }

        public IList<SpriteLayer> Items { get; set; } = [];
    }
}
