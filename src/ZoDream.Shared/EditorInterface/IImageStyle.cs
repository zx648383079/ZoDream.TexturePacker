using SkiaSharp;
using System.Numerics;

namespace ZoDream.Shared.EditorInterface
{
    public interface IImageStyle: IImageBound
    {

        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public float Rotate { get; set; }

        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
    }

    public interface IImageVertexStyle
    {
        /// <summary>
        /// 原图上的顶点 UV
        /// </summary>
        public Vector2[] VertexItems { get; }
        /// <summary>
        /// 实际绘制的顶点
        /// </summary>
        public SKPoint[] PointItems { get; }
    }

    public interface IImageComputedVertexStyle
    {
        /// <summary>
        /// 原图上的顶点
        /// </summary>
        public SKPoint[] SourceItems { get; }
        /// <summary>
        /// 实际绘制的顶点
        /// </summary>
        public SKPoint[] PointItems { get; }
    }

    public interface IImageComputedStyle : IImageStyle
    {

        public int ZIndex { get; set; }

        public float ActualLeft { get; }
        public float ActualTop { get; }

        public float ActualWidth { get; }
        public float ActualHeight { get; }

        public float ActualOuterWidth { get; }
        public float ActualOuterHeight { get; }
        /// <summary>
        /// 重新计算
        /// </summary>
        public void Compute();

    }
}
