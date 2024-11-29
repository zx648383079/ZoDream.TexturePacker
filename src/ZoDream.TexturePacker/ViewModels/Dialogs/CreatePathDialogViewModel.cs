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
    public class CreatePathDialogViewModel: BindableBase, 
        IFormValidator, ILayerCreator
    {
        public CreatePathDialogViewModel()
        {
            TextChangedCommand = new RelayCommand<bool>(OnTextChanged);
        }

        private string _text = string.Empty;

        public string Text {
            get => _text;
            set {
                Set(ref _text, value);
                IsValid = !string.IsNullOrWhiteSpace(value);
            }
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


        private bool _isValid;

        public bool IsValid {
            get => _isValid;
            set => Set(ref _isValid, value);
        }

        public ICommand TextChangedCommand { get; private set; }

        private void OnTextChanged(bool changed)
        {
            IsValid = changed;
        }

        public bool TryCreate(IImageEditor editor)
        {
            if (!IsValid)
            {
                return false;
            }
            editor.Add(new PathImageSource(SKPath.ParseSvgPathData(Text), editor)
            {
                StrokeColor = StrokeColor.ToSKColor(),
                StrokeWidth = StrokeWidth,
            });
            return true;
        }
    }
}
