using SkiaSharp;

namespace ZoDream.Plugin.Spine.Models
{
    internal class ColorTimeline: CurveTimeline
    {
        public ColorTimeline(int frameCount): base(frameCount)
        {
            Frames = new float[frameCount];
            ColorFrames = new SKColor[frameCount];
        }
        public int SlotIndex {  get; set; }
        public float[] Frames { get; set; }
        public SKColor[] ColorFrames { get; set; }

        public override int PropertyId => ((int)TimelineType.Color << 24) + SlotIndex;
    }
}
