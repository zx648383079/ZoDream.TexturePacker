namespace ZoDream.Plugin.Spine.Models
{
    public class ScaleTimeline : TranslateTimeline
    {
        public ScaleTimeline(int frameCount): base(frameCount)
        {
            
        }
        public override int PropertyId => ((int)TimelineType.ScaleX << 24) + BoneIndex;
    }
}
