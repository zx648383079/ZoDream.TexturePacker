using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ImageEditor;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel : IImageCommander
    {
        private readonly SKSizeI _thumbnailSize = new(60, 60);

        private readonly ImageStyleManager _styleManager = new();
        public IImageStyler Styler => LayerMode ? RealStyler : DefaultStyler;

        public IImageStyler DefaultStyler => _styleManager.Default;

        public IImageStyler RealStyler => _styleManager.Real;

        public IImageLayerTree Source => LayerItems;

        private bool _layerMode;

        public bool LayerMode {
            get => _layerMode;
            set {
                Set(ref _layerMode, value);
                Instance?.Invalidate();
            }
        }


        private LayerTree _layerItems = [];

        public LayerTree LayerItems {
            get => _layerItems;
            set => Set(ref _layerItems, value);
        }

        
        public IImageLayer? GetLayer(int id)
        {
            return LayerItems.Get(id);
        }

        public IImageLayer? GetLayer(string name)
        {
            return LayerItems.Get(item => item.Name == name);
        }


        public BitmapSource? CreateThumbnail(IImageSource source)
        {
            return source.CreateThumbnail(_thumbnailSize)?.ToWriteableBitmap();
        }

        public IImageLayer Create(IImageSource source)
        {
            return new LayerViewModel(this, source)
            {
                PreviewImage = CreateThumbnail(source)
            };
        }

        public IImageLayer Create(IImageSource source, string name)
        {
            return new LayerViewModel(this, source)
            {
                Name = name,
                PreviewImage = CreateThumbnail(source)
            };
        }
    }
}
