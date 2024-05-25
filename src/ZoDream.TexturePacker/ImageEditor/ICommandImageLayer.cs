using SkiaSharp;
using System;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface ICommandImageLayer: IImageLayer
    {
        public void Resize(int width, int height);

        public void Resize(IImageLayer layer);

        /// <summary>
        /// 依赖Editor尺寸的需要重绘
        /// </summary>
        public void Invalidate();
    }
}
