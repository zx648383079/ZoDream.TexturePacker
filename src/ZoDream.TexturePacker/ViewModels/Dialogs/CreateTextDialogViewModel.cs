using Microsoft.UI;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Windows.UI;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class CreateTextDialogViewModel: BindableBase, 
        IFormValidator, ILayerCreator
    {
        public CreateTextDialogViewModel()
        {
            FamilyName = SKTypeface.Default.FamilyName;
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

        private int _size = 16;

        public int Size {
            get => _size;
            set => Set(ref _size, value);
        }

        public string[] FamilyItems { get; set; } = SKFontManager.Default.FontFamilies.ToArray();

        private string _familyName = string.Empty;

        public string FamilyName {
            get => _familyName;
            set => Set(ref _familyName, value);
        }

        private Color _foreground = Colors.Black;

        public Color Foreground {
            get => _foreground;
            set => Set(ref _foreground, value);
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
            editor.AddText(Text, FamilyName, Size, Foreground.ToSKColor());
            return true;
        }
    }
}
