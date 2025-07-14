using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class SkeletonSection
    {
        public int FrameRate { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Width { get; set; }

        public float Height { get; set; }

        public IList<SkeletonBone> BoneItems { get; set; } = [];

        public IList<SkeletonSkin> SkinItems { get; set; } = [];
        public IList<SkeletonSlot> SlotItems { get; set; } = [];

        public IList<SkeletonAnimationSection> AnimationItems { get; set; } = [];

    }

}
