using SkiaSharp;
using System;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageBound: IImagePoint, IImageSize
    {
    }

    public interface IImagePoint
    {
        public int X { get; set; }

        public int Y { get; set; }

    }

    public interface IImageSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

    }
}
