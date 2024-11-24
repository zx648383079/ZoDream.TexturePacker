using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageComputedStyle: IImageComputedStyle
    {
        public ImageComputedStyle()
        {
            
        }

        public ImageComputedStyle(IImageStyle style)
            : this(style, style.RotateDeg)
        {
        }

        public ImageComputedStyle(IImageStyle style, float rotate)
        {
            X = style.X;
            Y = style.Y;
            Width = style.Width;
            Height = style.Height;
            RotateDeg = rotate % 360;
            Compute();
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public float RotateDeg { get; set; }

        public int ActualLeft => X;
        public int ActualTop => Y;

        public int ActualWidth { get; private set; }
        public int ActualHeight { get; private set; }

        public int ActualOuterWidth => ActualLeft + ActualWidth;
        public int ActualOuterHeight => ActualTop + ActualHeight;

        public void Compute()
        {
            if (RotateDeg == 0)
            {
                ActualWidth = Width;
                ActualHeight = Height;
                return;
            }
            (ActualWidth, ActualHeight) = 
                Drawing.SkiaExtension.ComputedRotate(Width, Height, RotateDeg);
        }
    }
}
