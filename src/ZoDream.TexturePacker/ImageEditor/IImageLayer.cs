using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using System;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageLayer: IImageBound, IDisposable
    {
        public int Id { get; set; }
        public int Depth { get; set; }

        public bool Visible { get; set; }

        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public float RotateDeg { get; set; }

        public void Rotate(float angle);

        public void Paint(SKCanvas canvas);

        public BitmapSource? GetPreviewSource();
    }
}
