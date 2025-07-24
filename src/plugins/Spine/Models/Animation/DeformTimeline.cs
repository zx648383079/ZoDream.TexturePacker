namespace ZoDream.Plugin.Spine.Models
{
    public class DeformTimeline : CurveTimeline
    {
        public DeformTimeline(int frameCount): base(frameCount)
        {
            Frames = new float[frameCount];
            Vertices = new float[frameCount][];
        }
        public int SlotIndex {  get; set; }
        public float[] Frames { get; set; }
        public float[][] Vertices { get; set; }
        public VertexAttachment Attachment { get; set; }

        public override int PropertyId => ((int)TimelineType.Deform << 24) + Attachment.Id + SlotIndex;
    }
}
