namespace ZoDream.Plugin.Spine.Models
{
    internal class ShearTimeline: TranslateTimeline
    {
        public ShearTimeline(int frameCount) : base(frameCount)
        {
            
        }
        public override int PropertyId => ((int)TimelineType.ShearX << 24) + BoneIndex;
    }
}
