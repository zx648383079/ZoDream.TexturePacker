using SkiaSharp;
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
        public float RotateDeg { get; set; }

        public abstract void Paint(IImageCanvas canvas, IImageStyle computedStyle);

        public virtual SKBitmap? CreateThumbnail(SKSizeI size)
        {
            return null;
        }
        public virtual void Dispose()
        {

        }

        public void Rotate(float angle)
        {
            RotateDeg += angle;
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
