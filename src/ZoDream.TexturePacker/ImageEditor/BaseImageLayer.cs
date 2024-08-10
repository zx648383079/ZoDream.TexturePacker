using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    public abstract class BaseImageLayer(IImageEditor editor) : IImageLayer
    {

        protected IImageEditor Editor { get; private set; } = editor;
        public int Id { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }

        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public float RotateDeg { get; set; }
        public bool Visible { get; set; } = true;
        public abstract void Paint(SKCanvas canvas);

        public virtual BitmapSource? GetPreviewSource()
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

        

    }
}
