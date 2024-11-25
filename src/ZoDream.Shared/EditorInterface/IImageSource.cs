using SkiaSharp;
using System;

namespace ZoDream.Shared.EditorInterface
{
    public interface IImageSource: IImageStyle, IDisposable
    {
        public bool Contains(float x, float y);

        public void Paint(IImageCanvas canvas, IImageStyle computedStyle);
        /// <summary>
        /// 生成预览图
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public SKBitmap? CreateThumbnail(SKSizeI size);
    }
}
