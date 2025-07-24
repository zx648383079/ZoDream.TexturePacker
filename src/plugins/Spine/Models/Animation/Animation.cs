using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Spine.Models
{
    public class Animation : ISkeletonAnimation
    {
        public string Name { get; set; }
        public Timeline[] Timelines { get; set; }
        public float Duration { get; set; }
    }
}
