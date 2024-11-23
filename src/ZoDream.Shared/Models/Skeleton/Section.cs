using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class SkeletonSection
    {
        public int FrameRate { get; set; }

        public string Name { get; set; } = string.Empty;

        public IList<SkeletonBone> BoneItems { get; set; } = [];

        public IList<SkeletonAnimationSection> AnimationItems { get; set; } = [];

    }

}
