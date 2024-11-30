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

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public float Rotate { get; set; }

        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;

        public float ShearX { get; set; }
        public float ShearY { get; set; }

        public int ZIndex { get; set; }

        public float ActualLeft => X;
        public float ActualTop => Y;

        public float ActualWidth { get; private set; }
        public float ActualHeight { get; private set; }

        public float ActualOuterWidth => ActualLeft + ActualWidth;
        public float ActualOuterHeight => ActualTop + ActualHeight;

        public void Compute()
        {
            var width = (float)(Width * Math.Abs(ScaleX));
            var height = (float)(Height * Math.Abs(ScaleY));
            if (Rotate == 0)
            {
                ActualWidth = width;
                ActualHeight = height;
                return;
            }
            (ActualWidth, ActualHeight) = 
                EditorExtension.ComputedRotate(width, height, Rotate);
        }
    }
}
