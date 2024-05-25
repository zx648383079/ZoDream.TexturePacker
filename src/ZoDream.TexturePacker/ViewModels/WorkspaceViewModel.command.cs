using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.Storage;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Plugins;

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
            var (width, height) = new CssSprites().Compute(Editor.LayerItems.ToArray<IImageBound>());
            Editor.Resize(width, height);
            Editor.Invalidate();
        }

    }
}
