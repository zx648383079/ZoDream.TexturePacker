using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal class TranslateTimeline: CurveTimeline
    {
        public TranslateTimeline(int frameCount): base(frameCount)
        {
            Frames = new float[frameCount];
            Points = new SKPoint[frameCount];
        }
        public int BoneIndex { get; set; }
        public float[] Frames { get; set; }

        public SKPoint[] Points { get; set; }

        public override int PropertyId => ((int)TimelineType.Translate << 24) + BoneIndex;
    }
}
