using SkiaSharp;
using System.Numerics;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageComputedVertexStyle(int layerId) : 
        ImageComputedStyle(layerId), IImageComputedVertexStyle
    {
        /// <summary>
        /// 原图上的顶点
        /// </summary>
        public SKPoint[] SourceItems { get; set; } = [];
        /// <summary>
        /// 实际绘制的顶点
        /// </summary>
        public SKPoint[] PointItems { get; set; } = [];
    }
}
