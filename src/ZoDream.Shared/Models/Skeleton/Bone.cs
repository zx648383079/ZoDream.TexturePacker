using System.Collections.Generic;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Models
{
    public class SkeletonBone : IReadOnlyStyle
    {
        public string Name { get; set; } = string.Empty;

        public string Parent { get; set; } = string.Empty;

        public float Length { get; set; }


        public float X { get; set; }
        public float Y { get; set; }

        public float Rotate { get; set; }

        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ShearX { get; set; }
        public float ShearY { get; set; }
        public IList<SpriteLayer> SkinItems { get; set; } = [];
    }

}
