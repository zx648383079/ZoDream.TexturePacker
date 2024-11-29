using Microsoft.UI;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System.Windows.Input;
using Windows.UI;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ImageEditor;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class CreateRectDialogViewModel: BindableBase, 
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

        private float _width;

        public float Width {
            get => _width;
            set => Set(ref _width, value);
        }

        private float _height;

        public float Height {
            get => _height;
            set => Set(ref _height, value);
        }

        private float _leftRadius;

        public float LeftRadius {
            get => _leftRadius;
            set => Set(ref _leftRadius, value);
        }
        private float _topRadius;

        public float TopRadius {
            get => _topRadius;
            set => Set(ref _topRadius, value);
        }
        private float _rightRadius;

        public float RightRadius {
            get => _rightRadius;
            set => Set(ref _rightRadius, value);
        }
        private float _bottomRadius;

        public float BottomRadius {
            get => _bottomRadius;
            set => Set(ref _bottomRadius, value);
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

        public bool IsValid => Width == 0 || Height == 0;

        public bool TryCreate(IImageEditor editor)
        {
            if (!IsValid)
            {
                return false;
            }
            editor.Add(new RectImageSource(editor)
            {
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                FillColor = FillColor.ToSKColor(),
                StrokeColor = StrokeColor.ToSKColor(),
                StrokeWidth = StrokeWidth,
                LeftRadius = LeftRadius,
                TopRadius = TopRadius,
                RightRadius = RightRadius,
                BottomRadius = BottomRadius,
            });
            return true;
        }
    }
}
