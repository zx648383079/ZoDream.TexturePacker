using System;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageComputedStyle: IImageComputedStyle
    {
        public ImageComputedStyle()
        {
            
        }

        public ImageComputedStyle(int layerId)
        {
            LayerId = layerId;
        }

        public ImageComputedStyle(IImageStyle style)
            : this(style, style.Rotate)
        {
        }

        public ImageComputedStyle(IImageStyle style, float rotate)
        {
            X = style.X;
            Y = style.Y;
            Width = style.Width;
            Height = style.Height;
            Rotate = EditorExtension.ConvertAngle(rotate);
            ScaleX = style.ScaleX;
            ScaleY = style.ScaleY;
            Compute();
        }

        public int LayerId { get; private set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public float Rotate { get; set; }

        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;

        public int ZIndex { get; set; }

        public int ActualLeft => X;
        public int ActualTop => Y;

        public int ActualWidth { get; private set; }
        public int ActualHeight { get; private set; }

        public int ActualOuterWidth => ActualLeft + ActualWidth;
        public int ActualOuterHeight => ActualTop + ActualHeight;

        public void Compute()
        {
            var width = (int)(Width * Math.Abs(ScaleX));
            var height = (int)(Height * Math.Abs(ScaleY));
            if (Rotate == 0)
            {
                ActualWidth = width;
                ActualHeight = height;
                return;
            }
            (ActualWidth, ActualHeight) = 
                Drawing.SkiaExtension.ComputedRotate(width, height, Rotate);
        }
    }
}
