using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.TexturePacker.Dialogs;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private async void TapLayerRename(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var dialog = new RenameDialog();
            dialog.ViewModel.Name = layer.Name;
            var res = await App.ViewModel.OpenDialogAsync(dialog);
            if (res != ContentDialogResult.Primary || string.IsNullOrWhiteSpace(dialog.ViewModel.Name))
            {
                return;
            }
            layer.Name = dialog.ViewModel.Name;
        }

        private async void TapLayerProperty(object? _)
        {
            var dialog = new LayerPropertyDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }
    }
}
