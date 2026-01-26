using Microsoft.UI;
using System.Windows.Input;
using Windows.UI;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class CreateProjectDialogViewModel: BindableBase
    {
        public CreateProjectDialogViewModel()
        {
            SizeRestoreCommand = new RelayCommand(TapSizeRestore);
        }

        private string _name = string.Empty;

        public string Name {
            get => _name;
            set => Set(ref _name, value);
        }

        private bool _isLockSize;

        public bool IsLockSize {
            get => _isLockSize;
            set => Set(ref _isLockSize, value);
        }

        private int _width;

        public int Width {
            get => _width;
            set => Set(ref _width, value);
        }

        private int _height;

        public int Height {
            get => _height;
            set => Set(ref _height, value);
        }

        private bool _isEditableSize;

        public bool IsEditableSize {
            get => _isEditableSize;
            set => Set(ref _isEditableSize, value);
        }

        private bool _isTransparentBackground = true;

        public bool IsTransparentBackground {
            get => _isTransparentBackground;
            set => Set(ref _isTransparentBackground, value);
        }

        private Color _foreground = Colors.White;

        public Color Foreground {
            get => _foreground;
            set => Set(ref _foreground, value);
        }

        private Color _background = Colors.Black;

        public Color Background {
            get => _background;
            set => Set(ref _background, value);
        }

        public bool SizeRestoreEnabled => !IsEditableSize;

        public ICommand SizeRestoreCommand { get; private set; }

        private void TapSizeRestore(object? _)
        {
            IsEditableSize = false;
        }

        public void Load(IImageEditor editor)
        {
            //Name = layer.Name;
            Width = editor.ActualWidthI;
            Height = editor.ActualHeightI;
        }

        public void Save(IImageEditor layer)
        {

        }
    }
}
