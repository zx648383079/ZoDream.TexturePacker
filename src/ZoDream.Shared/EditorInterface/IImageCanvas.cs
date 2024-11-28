using SkiaSharp;
using System;

namespace ZoDream.Shared.EditorInterface
{
    public interface IImageCanvas : IImagePoint
    {
        public IImageCanvas Transform(float x, float y);
        public void Mutate(IImageStyle style, Action<IImageCanvas> cb);
        public IImageStyle Compute(IImageLayer layer);
        public void DrawBitmap(SKBitmap? source, IImageStyle style);
        public void DrawSurface(SKSurface? surface, IImageStyle style);
        public void DrawSurface(SKSurface? surface, float x, float y);
        public void DrawPicture(SKPicture? picture, IImageStyle style);
        public void DrawText(string text, IImageStyle style, SKTextAlign textAlign, SKFont font, SKPaint paint);

        public void DrawPath(SKPath path, SKPaint paint);
        /// <summary>
        /// 绘制纹理
        /// </summary>
        /// <param name="source">纹理图片</param>
        /// <param name="sourceVertices">纹理上的顶点</param>
        /// <param name="vertices">顶点对于的位置</param>
        public void DrawTexture(SKBitmap source, SKPoint[] sourceVertices, SKPoint[] vertices);
        /// <summary>
        /// 画矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="paint"></param>
        public void DrawRect(SKRect rect, SKPaint paint);
        /// <summary>
        /// 画圆角矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="paint"></param>
        public void DrawRect(SKRoundRect rect, SKPaint paint);
        /// <summary>
        /// 画圆
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="paint"></param>
        public void DrawCircle(float x, float y, float radius, SKPaint paint);
        /// <summary>
        /// 画椭圆
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xRadius"></param>
        /// <param name="yRadius"></param>
        /// <param name="paint"></param>
        public void DrawOval(float x, float y, float xRadius, float yRadius, SKPaint paint);
    }
}
