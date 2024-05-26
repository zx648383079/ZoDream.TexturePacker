using System;
using System.Collections.ObjectModel;
using System.Linq;
using ZoDream.TexturePacker.Models;

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
    }
}
