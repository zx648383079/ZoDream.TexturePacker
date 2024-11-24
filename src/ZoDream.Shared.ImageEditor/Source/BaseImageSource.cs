﻿using SkiaSharp;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public abstract class BaseImageSource(IImageEditor editor) : IImageSource
    {

        protected IImageEditor Editor { get; private set; } = editor;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public float Rotate { get; set; }

        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;

        public abstract void Paint(IImageCanvas canvas, IImageStyle computedStyle);

        public virtual SKBitmap? CreateThumbnail(SKSizeI size)
        {
            return null;
        }
        public virtual void Dispose()
        {

        }

        public virtual bool Contains(float x, float y)
        {
            var offsetX = x - X;
            if (offsetX < 0 || offsetX > Width)
            {
                return false;
            }
            var offsetY = y - Y;
            if (offsetY < 0 || offsetY > Height)
            {
                return false;
            }
            return true;
        }
    }
}
