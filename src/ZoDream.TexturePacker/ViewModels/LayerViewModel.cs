using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using ZoDream.Shared.ViewModel;
using ZoDream.TexturePacker.Models;

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

        private ObservableCollection<LayerViewModel> _children = [];

        public ObservableCollection<LayerViewModel> Children {
            get => _children;
            set => Set(ref _children, value);
        }

        public LayerViewModel? Get(int id)
        {
            if (id == Id)
            {
                return this;
            }
            foreach (var item in Children)
            {
                var layer = item.Get(id);
                if (layer is not null)
                {
                    return layer;
                }
            }
            return null;
        }
    }
}
