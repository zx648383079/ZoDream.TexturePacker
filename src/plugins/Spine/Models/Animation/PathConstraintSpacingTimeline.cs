namespace ZoDream.Plugin.Spine.Models
{
    public class PathConstraintSpacingTimeline : PathConstraintPositionTimeline
    {
        public PathConstraintSpacingTimeline(int frameCount): base(frameCount)
        {
            
        }
        public override int PropertyId => ((int)TimelineType.PathConstraintSpacing << 24) + PathConstraintIndex;
    }
}
