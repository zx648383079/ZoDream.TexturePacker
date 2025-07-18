using System.Collections.Generic;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Models
{

    public class SkeletonAnimationSection : ISkeletonAnimation
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

}
