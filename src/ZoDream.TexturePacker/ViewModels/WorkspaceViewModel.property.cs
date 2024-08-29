using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
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

        private LayerTree _layerItems = [];

        public LayerTree LayerItems {
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
            return LayerItems.Get(id);
        }

        public LayerViewModel? GetLayer(string name)
        {
            return LayerItems.Get(item => item.Name == name);
        }

        public void AddLayer(SpriteLayerSection data)
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

        public LayerViewModel AddLayer(int id, string name, BitmapSource? image)
        {
            var layer = new LayerViewModel()
            {
                Id = id,
                Name = name,
                PreviewImage = image
            };
            LayerItems.Insert(0, layer);
            return layer;
        }
    }
}
