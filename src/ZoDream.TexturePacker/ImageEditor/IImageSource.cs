using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using System;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageSource: IImageBound, IDisposable
    {
        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public float RotateDeg { get; set; }

        public void Rotate(float angle);

        public bool Contains(float x, float y);

        public void Paint(IImageCanvas canvas);

        public BitmapSource? GetPreviewSource();
    }
}
