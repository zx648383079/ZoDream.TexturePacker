namespace ZoDream.Plugin.Spine.Models
{
    public class DrawOrderTimeline : Timeline
    {
        public DrawOrderTimeline(int frameCount)
        {
            Frames = new float[frameCount];
            DrawOrders = new int[frameCount][];
        }
        public float[] Frames { get; set; } // time, ...
        public int[][] DrawOrders { get; set; }
        public int FrameCount => Frames.Length;

        public int PropertyId => ((int)TimelineType.DrawOrder << 24);
    }
}
