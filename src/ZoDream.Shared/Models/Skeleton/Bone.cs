using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class SkeletonBone
    {
        public string Name { get; set; } = string.Empty;

        public string Parent { get; set; } = string.Empty;

        public float Length { get; set; }


        public float X { get; set; }
        public float Y { get; set; }

        public float Rotate { get; set; }

        public IList<SkeletonBoneTexture> SkinItems { get; set; } = [];
    }

    public class SkeletonBoneTexture
    {
        public string Name { get; set; } = string.Empty;

        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public float Rotate { get; set; }
    }

}
