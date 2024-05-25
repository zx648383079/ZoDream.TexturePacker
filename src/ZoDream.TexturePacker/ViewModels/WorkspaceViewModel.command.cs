using System.Collections.Generic;
using System.Windows.Input;
using Windows.Storage;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {


        public ICommand DragImageCommand { get; private set; }

        private void OnDragImage(IReadOnlyList<IStorageItem> items)
        {
            foreach (var item in items)
            {
                Editor.AddImage(item.Path);
            }
            Editor.Invalidate();
        }

    }
}
