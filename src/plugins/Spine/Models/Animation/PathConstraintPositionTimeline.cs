namespace ZoDream.Plugin.Spine.Models
{
    public class PathConstraintPositionTimeline : CurveTimeline
    {
        public PathConstraintPositionTimeline(int frameCount): base(frameCount)
        {
            Frames = new float[frameCount];
            PositionItems = new float[frameCount];
        }
        public int PathConstraintIndex {  get; set; }
        public float[] Frames { get; set; } // time, position, ...

        public float[] PositionItems { get; set; }

        public override int PropertyId => ((int)TimelineType.PathConstraintPosition << 24) + PathConstraintIndex;
    }
}
