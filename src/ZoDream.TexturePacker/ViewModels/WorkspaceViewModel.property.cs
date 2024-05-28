using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ZoDream.Shared.UndoRedo;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        public CommandManager UndoRedo { get; private set; } = new();
        public IImageEditor? Editor { get; set; }

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

        private ObservableCollection<LayerViewModel> _layerItems = [];

        public ObservableCollection<LayerViewModel> LayerItems {
            get => _layerItems;
            set => Set(ref _layerItems, value);
        }

        private LayerViewModel? _selectedLayer;

        public LayerViewModel? SelectedLayer {
            get => _selectedLayer;
            set {
                Set(ref _selectedLayer, value);
                OnPropertyChanged(nameof(IsSelectedLayer));
            }
        }


        public LayerViewModel? GetLayer(int id)
        {
            foreach (var item in LayerItems)
            {
                var layer = item.Get(id);
                if (layer is not null)
                {
                    return layer;
                }
            }
            return null;
        }

        public void AddLayer(LayerGroupItem data)
        {
            LayerItems.Add(new LayerViewModel()
            {
                Name = data.Name,
                Children = [..data.Items.Select(item => new LayerViewModel()
                {
                    Name = item.Name
                })]
            });
        }

        public void AddLayer(int id, string name, BitmapSource? image)
        {
            LayerItems.Insert(0, new LayerViewModel()
            {
                Id = id,
                Name = name,
                PreviewImage = image
            });
        }
    }
}
