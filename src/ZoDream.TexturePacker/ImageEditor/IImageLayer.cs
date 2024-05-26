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
        public void Paint(SKCanvas canvas);

        public BitmapSource? GetPreviewSource();
    }
}
