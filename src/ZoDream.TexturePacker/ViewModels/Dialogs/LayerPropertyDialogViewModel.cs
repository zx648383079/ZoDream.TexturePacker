using ExCSS;
using System.Windows.Input;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class LayerPropertyDialogViewModel: BindableBase, IFormValidator
    {
        public LayerPropertyDialogViewModel()
        {
            OffsetRestoreCommand = new RelayCommand(TapOffsetRestore);
            ScaleRestoreCommand = new RelayCommand(TapScaleRestore);
            RotateRestoreCommand = new RelayCommand(TapRotateRestore);
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

        private bool _isLockScale;

        public bool IsLockScale {
            get => _isLockScale;
            set => Set(ref _isLockScale, value);
        }

        private int _x;

        public int X {
            get => _x;
            set {
                Set(ref _x, value);
                OnPropertyChanged(nameof(OffsetRestoreEnabled));
            }
        }

        private int _y;

        public int Y {
            get => _y;
            set {
                Set(ref _y, value);
                OnPropertyChanged(nameof(OffsetRestoreEnabled));
            }
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


        private double _scaleX = 1;

        public double ScaleX {
            get => _scaleX;
            set {
                Set(ref _scaleX, value);
                OnPropertyChanged(nameof(ScaleRestoreEnabled));
            }
        }

        private double _scaleY = 1;

        public double ScaleY {
            get => _scaleY;
            set {
                Set(ref _scaleY, value);
                OnPropertyChanged(nameof(ScaleRestoreEnabled));
            }
        }

        private double _rotate;

        public double Rotate {
            get => _rotate;
            set {
                Set(ref _rotate, value);
                OnPropertyChanged(nameof(RotateRestoreEnabled));
            }
        }


        private bool _isVisible = true;

        public bool IsVisible {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        private bool _isLocked;

        public bool IsLocked {
            get => _isLocked;
            set => Set(ref _isLocked, value);
        }

        public bool OffsetRestoreEnabled => X != 0 || Y != 0;
        public bool ScaleRestoreEnabled => ScaleX != 1 || ScaleY != 1;
        public bool RotateRestoreEnabled => Rotate != 0;

        public bool IsValid => true;

        public ICommand OffsetRestoreCommand { get; private set; }
        public ICommand ScaleRestoreCommand { get; private set; }
        public ICommand RotateRestoreCommand { get; private set; }

        private void TapOffsetRestore(object? _)
        {
            X = 0;
            Y = 0;
        }

        private void TapScaleRestore(object? _)
        {
            ScaleX = 1;
            ScaleY = 1;
        }
        private void TapRotateRestore(object? _)
        {
            Rotate = 0;
        }

        public void Load(IImageLayer layer)
        {
            Name = layer.Name;
            X = (int)layer.Source.X;
            Y = (int)layer.Source.Y;
            Width = (int)layer.Source.Width;
            Height = (int)layer.Source.Height;
            ScaleX = layer.Source.ScaleX;
            ScaleY = layer.Source.ScaleY;
            Rotate = layer.Source.Rotate;
            IsVisible = layer.IsVisible;
            IsLocked = layer.IsLocked;
        }

        public void Save(IImageLayer layer)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                layer.Name = Name;
            }
            layer.Source.X = X;
            layer.Source.Y = Y;
            if (Width > 0) 
            {
                layer.Source.Width = Width;
            }
            if (Height > 0)
            {
                layer.Source.Height = Height;
            }
            
            if (ScaleX != 0)
            {
                layer.Source.ScaleX = (float)ScaleX;
            }
            if (ScaleY != 0) 
            {
                layer.Source.ScaleY = (float)ScaleY;
            }
            layer.Source.Rotate = (float)Rotate;
            layer.IsVisible = IsVisible;
            layer.IsLocked = IsLocked;
        }
    }
}
