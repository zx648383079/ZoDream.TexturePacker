using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.TexturePacker.Plugins.Readers.Spine
{
    public class SP_SkeletonRoot
    {
        public Dictionary<string, SP_Animation> Animations { get; set; }

        public SP_Bone[] Bones { get; set; }

        public SP_Skeleton Skeleton { get; set; }

        public SP_Skin[] Skins { get; set; }

        public SP_Slot[] Slots { get; set; }
    }


    public class SP_Animation
    {
        public Dictionary<string, SP_AnimationBone> Bones { get; set; }
    }

    public class SP_AnimationBone: Dictionary<string, SP_AnimationFrame[]>
    {
    }

    public class SP_AnimationFrame
    {
        public float? Angle { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }

        public float[] Curve { get; set; }

        public float Time { get; set; }


    }


    public class SP_Bone
    {
        public string Name { get; set; }

        public float? Length { get; set; }

        public string Parent { get; set; }

        public float? Rotation { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
    }

    public class SP_Skeleton
    {
        public string Hash { get; set; }

        public double Height { get; set; }

        public string Images { get; set; }

        public string Spine { get; set; }

        public double Width { get; set; }
    }

    public class SP_Skin
    {
        public Dictionary<string, SP_Attachment> Attachments { get; set; }

        public string Name { get; set; }
    }

    public class SP_Attachment : Dictionary<string, SP_SkinProperty> 
    {

    }

    public class SP_SkinProperty
    {
        public float Height { get; set; }

        public float? Rotation { get; set; }

        public float Width { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public long[]? Edges { get; set; }

        public long Hull { get; set; }

        public long[]? Triangles { get; set; }

        public string Type { get; set; }

        public double[]? Uvs { get; set; }

        public double[]? Vertices { get; set; }

    }

    public class SP_Slot
    {
        public string Attachment { get; set; }

        public string Bone { get; set; }

        public string Name { get; set; }
    }
}
