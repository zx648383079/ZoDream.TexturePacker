using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal class Bone
    {
        public string Name { get; set; }

        public float? Length { get; set; }

        public string Parent { get; set; }

        public float? Rotation { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ShearX { get; set; }
        public float ShearY { get; set; }

        public TransformMode TransformMode { get; set; }
    }
}
