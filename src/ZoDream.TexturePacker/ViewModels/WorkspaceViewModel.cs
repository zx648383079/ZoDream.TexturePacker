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
            DragImageCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnDragImage);
        }
    }
}
