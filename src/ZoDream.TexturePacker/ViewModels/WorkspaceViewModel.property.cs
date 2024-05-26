using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ZoDream.TexturePacker.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {

        public ImageEditor.Editor Editor { get; private set; } = new(300, 700);


        private ObservableCollection<LayerViewModel> _layerItems = [];

        public ObservableCollection<LayerViewModel> LayerItems {
            get => _layerItems;
            set => Set(ref _layerItems, value);
        }

        private LayerViewModel? _selectedLayer;

        public LayerViewModel? SelectedLayer {
            get => _selectedLayer;
            set => Set(ref _selectedLayer, value);
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
