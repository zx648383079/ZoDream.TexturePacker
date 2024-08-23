using Microsoft.UI.Xaml.Media.Imaging;
using System;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class LayerViewModel: BindableBase
    {
        public int Id { get; set; }

        private string _name = string.Empty;

        public string Name {
            get => _name;
            set => Set(ref _name, value);
        }

        private BitmapSource? _previewImage;

        public BitmapSource? PreviewImage {
            get => _previewImage;
            set => Set(ref _previewImage, value);
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

        private LayerTree _children = [];

        public LayerTree Children {
            get => _children;
            set => Set(ref _children, value);
        }

        public LayerViewModel? Get(int id)
        {
            return Get(item => item.Id == id);
        }

        public LayerViewModel? Get(Func<LayerViewModel, bool> checkFn)
        {
            if (checkFn(this))
            {
                return this;
            }
            return Children.Get(checkFn);
        }
    }
}
