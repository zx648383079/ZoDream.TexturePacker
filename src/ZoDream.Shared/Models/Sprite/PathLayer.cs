using SkiaSharp;
using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class SpritePathLayer: SpriteLayer
    {
        /// <summary>
        /// 实际绘制的顶点
        /// </summary>
        public IList<SKPoint> PointItems { get; set; } = [];

        public SKColor FillColor { get; set; }
        public SKColor StrokeColor { get; set; }

        public float StrokeWidth { get; set; }
    }

}
