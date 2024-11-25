using Microsoft.UI.Xaml.Controls;
using System;
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
            var res = await _app.OpenDialogAsync(dialog);
            if (res != ContentDialogResult.Primary || string.IsNullOrWhiteSpace(dialog.ViewModel.Name))
            {
                return;
            }
            layer.Name = dialog.ViewModel.Name;
        }

        private async void TapLayerProperty(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var dialog = new LayerPropertyDialog();
            var model = dialog.ViewModel;
            model.Load(layer);
            var res = await _app.OpenFormAsync(dialog);
            if (!res)
            {
                return;
            }
            model.Save(layer);
        }
    }
}
