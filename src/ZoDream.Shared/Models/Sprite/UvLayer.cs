using SkiaSharp;
using System.Collections.Generic;
using System.Numerics;

namespace ZoDream.Shared.Models
{
    public class SpriteUvLayer: SpriteLayer
    {
        /// <summary>
        /// 原图上的顶点 UV
        /// </summary>
        public IList<Vector2> VertexItems { get; set; } = [];
        /// <summary>
        /// 实际绘制的顶点
        /// </summary>
        public IList<SKPoint> PointItems { get; set; } = [];
    }

}
