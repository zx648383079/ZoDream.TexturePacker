using System.Collections.Generic;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Models
{
    public class SkeletonSection : ISkeleton
    {
        public int FrameRate { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Width { get; set; }

        public float Height { get; set; }

        public IList<SkeletonBone> Bones { get; set; } = [];

        public IList<SkeletonSkin> Skins { get; set; } = [];
        public IList<SkeletonSlot> Slots { get; set; } = [];

        public IList<SkeletonAnimationSection> Animations { get; set; } = [];

        IEnumerable<ISkeletonBone> ISkeleton.Bones => Bones;

        IEnumerable<ISkeletonSkin> ISkeleton.Skins => Skins;

        IEnumerable<ISkeletonSlot> ISkeleton.Slots => Slots;

        IEnumerable<ISkeletonAnimation> ISkeleton.Animations => Animations;
    }

}
