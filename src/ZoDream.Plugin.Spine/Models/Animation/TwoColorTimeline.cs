using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal class TwoColorTimeline: CurveTimeline
    {
        public TwoColorTimeline(int frameCount): base(frameCount)
        {
            Frames = new float[frameCount];
            ColorFrames = new SKColor[frameCount];
            Color2Frames = new SKColor[frameCount];
        }
        public int SlotIndex { get; set; }
        public float[] Frames { get; set; }// time, r, g, b, a, r2, g2, b2, ...

        public SKColor[] ColorFrames { get; set; }
        public SKColor[] Color2Frames { get; set; }

        public override int PropertyId => ((int)TimelineType.TwoColor << 24) + SlotIndex;
    }
}
