using Microsoft.UI.Xaml.Media.Imaging;
using System.Linq;
using ZoDream.Shared.UndoRedo;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel : IImageCommander
    {
        public CommandManager UndoRedo { get; private set; } = new();
        public IImageEditor? Instance { get; set; }

        public IImageLayerTree Source => LayerItems;

        private bool _layerMode;

        public bool LayerMode {
            get => _layerMode;
            set => Set(ref _layerMode, value);
        }



        private bool _undoEnabled;

        public bool UndoEnabled {
            get => _undoEnabled;
            set => Set(ref _undoEnabled, value);
        }

        private bool _redoEnabled;

        public bool RedoEnabled {
            get => _redoEnabled;
            set => Set(ref _redoEnabled, value);
        }

        public bool IsSelectedLayer => SelectedLayer != null;

        private LayerTree _layerItems = [];

        public LayerTree LayerItems {
            get => _layerItems;
            set => Set(ref _layerItems, value);
        }

        private IImageLayer? _selectedLayer;

        public IImageLayer? SelectedLayer {
            get => _selectedLayer;
            set {
                Set(ref _selectedLayer, value);
                OnPropertyChanged(nameof(IsSelectedLayer));
            }
        }

        

        public IImageLayer? GetLayer(int id)
        {
            return LayerItems.Get(id);
        }

        public IImageLayer? GetLayer(string name)
        {
            return LayerItems.Get(item => item.Name == name);
        }


        public IImageLayer Create(IImageSource source)
        {
            return new LayerViewModel(this, source)
            {
                PreviewImage = source.GetPreviewSource()
            };
        }

        public IImageLayer Create(IImageSource source, string name)
        {
            return new LayerViewModel(this, source)
            {
                Name = name,
                PreviewImage = source.GetPreviewSource()
            };
        }
    }
}
