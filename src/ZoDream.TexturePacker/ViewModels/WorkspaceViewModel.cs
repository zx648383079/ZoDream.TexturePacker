using System.Collections.Generic;
using Windows.Storage;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel: BindableBase
    {
        public WorkspaceViewModel()
        {
            ExitCommand = new RelayCommand(TapExit);
            LayerSelectedCommand = new RelayCommand<LayerViewModel>(OnLayerSelected);
            DragImageCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnDragImage);
            Editor.SelectionChanged += Editor_SelectionChanged;
        }

        private void Editor_SelectionChanged(int id)
        {
            SelectedLayer = GetLayer(id);
        }
    }
}
