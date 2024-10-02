using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class LayerPropertyDialogViewModel: BindableBase
    {

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
            set => Set(ref _x, value);
        }

        private int _y;

        public int Y {
            get => _y;
            set => Set(ref _y, value);
        }

        private int _width;

        public int Width {
            get => _width;
            set => Set(ref _width, value);
        }

        private double _scaleX;

        public double ScaleX {
            get => _scaleX;
            set => Set(ref _scaleX, value);
        }

        private double _scaleY;

        public double ScaleY {
            get => _scaleY;
            set => Set(ref _scaleY, value);
        }

        private double _rotate;

        public double Rotate {
            get => _rotate;
            set => Set(ref _rotate, value);
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
    }
}
