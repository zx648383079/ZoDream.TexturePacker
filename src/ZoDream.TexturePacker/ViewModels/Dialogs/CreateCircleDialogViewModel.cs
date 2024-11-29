using Microsoft.UI;
using SkiaSharp.Views.Windows;
using Windows.UI;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ImageEditor;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class CreateCircleDialogViewModel: BindableBase, 
        IFormValidator, ILayerCreator
    {
        private float _x;

        public float X {
            get => _x;
            set => Set(ref _x, value);
        }

        private float _y;

        public float Y {
            get => _y;
            set => Set(ref _y, value);
        }

        private float _xRadius;

        public float XRadius {
            get => _xRadius;
            set => Set(ref _xRadius, value);
        }
        private float _yRadius;

        public float YRadius {
            get => _yRadius;
            set => Set(ref _yRadius, value);
        }
        
        private float _strokeWidth = 1;

        public float StrokeWidth {
            get => _strokeWidth;
            set => Set(ref _strokeWidth, value);
        }

        private Color _strokeColor = Colors.Black;

        public Color StrokeColor {
            get => _strokeColor;
            set => Set(ref _strokeColor, value);
        }
        private Color _fillColor = Colors.Black;

        public Color FillColor {
            get => _fillColor;
            set => Set(ref _fillColor, value);
        }

        public bool IsValid => XRadius > 0 && YRadius > 0;

        public bool TryCreate(IImageEditor editor)
        {
            if (!IsValid)
            {
                return false;
            }
            editor.Add(new CircleImageSource(editor)
            {
                X = X,
                Y = Y,
                Width = XRadius * 2,
                Height = YRadius * 2,
                FillColor = FillColor.ToSKColor(),
                StrokeColor = StrokeColor.ToSKColor(),
                StrokeWidth = StrokeWidth,
                XRadius = XRadius,
                YRadius = YRadius,
            });
            return true;
        }
    }
}
