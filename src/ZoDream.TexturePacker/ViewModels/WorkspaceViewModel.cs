using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel: BindableBase
    {
        public WorkspaceViewModel()
        {
            DragImageCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnDragImage);
        }
    }
}
