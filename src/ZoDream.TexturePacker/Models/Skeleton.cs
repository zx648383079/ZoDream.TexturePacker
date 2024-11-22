using System.Collections.Generic;

namespace ZoDream.TexturePacker.Models
{
    public class SkeletonSection
    {
        public int FrameRate { get; set; }

        public string Name { get; set; } = string.Empty;

        public IList<SkeletonBone> BoneItems { get; set; } = [];

        public IList<SkeletonAnimationSection> AnimationItems { get; set; } = [];

    }

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

    public class SkeletonAnimationSection 
    {
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int PayTimes { get; set; }

        public IList<SkeletonBoneAnimation> BoneItems { get; set; } = [];
    }

    public class SkeletonBoneAnimation
    {
        public string Name { get; set; } = string.Empty;

        public IList<SkeletonAnimationFrame> FrameItems { get; set; } = [];
    }

    public class SkeletonAnimationFrame
    {
        public string PropertyName { get; set; } = string.Empty;
        public float Duration { get; set; }
        public SkeletonEasing Easing { get; set; }

        public IList<float> CurveItems { get; set; } = [];

        public float TargetValue { get; set; }
    }

    public enum SkeletonEasing 
    {
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InBack,
        OutBack,
        InOutBack,
        InElastic,
        OutElastic,
        InOutElastic,
        InBounce,
        OutBounce,
        InOutBounce,

        Curve = 99,
    }
}
