using System.Collections.ObjectModel;

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

    }
}
