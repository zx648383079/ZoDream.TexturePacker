using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using ZoDream.TexturePacker.Dialogs;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {

        private void TapSave(object? _)
        {

        }
        private void TapSaveAs(object? _)
        {

        }
        private async void TapExport(object? _)
        {
            var dialog = new ExportDialog();
            if (await App.ViewModel.OpenDialogAsync(dialog) != ContentDialogResult.Primary)
            {
                return;
            }
            var fileName = await dialog.ViewModel.OpenPickerAsync();
            if (fileName is null) 
            {
                return;
            }
            switch (dialog.ViewModel.SourceIndex)
            {
                case 1:
                    ExportEveryLayer(fileName, dialog.ViewModel.LayerFolder);
                    break;
                case 2:
                    ExportSelected(fileName);
                    break;
                default:
                    ExportWhole(fileName);
                    break;
            }
        }

        private void ExportSelected(IStorageItem file)
        {
            if (SelectedLayer is null)
            {
                return;
            }
            var layer = Instance?.Get(SelectedLayer.Id);
            if (layer == null) 
            {
                return;
            }
            var fileName = file.Path;
            if (file is StorageFolder) 
            {
                fileName = CombineLayerPath(file.Path, SelectedLayer.Name);
            }
            Instance?.SaveAs(layer, fileName);
            App.ViewModel.Toast.Show("导出完成");
        }

        private string CombineLayerPath(string root, string name)
        {
            if (!name.Contains('.'))
            {
                name += ".png";
            }
            return Path.Combine(root, name);
        }

        private void ExportEveryLayer(IStorageItem file, bool autoLayerFolder)
        {
            var root = file.Path;
            if (file is IStorageFile)
            {
                root = Path.GetDirectoryName(file.Path);
            }
            foreach (var item in LayerItems)
            {
                var layerFolder = root;
                if (!item.IsVisible) 
                {
                    continue;
                }
                if (item.Children.Count == 0)
                {
                    Instance?.SaveAs(item,
                        CombineLayerPath(layerFolder, item.Name));
                    continue;
                }
                if (autoLayerFolder)
                {
                    layerFolder = Path.Combine(layerFolder, item.Name);
                    if (!Directory.Exists(layerFolder)) 
                    {
                        Directory.CreateDirectory(layerFolder);
                    }
                }
                foreach (var it in item.Children)
                {
                    if (!it.IsVisible)
                    {
                        continue;
                    }
                    Instance?.SaveAs(it,
                        CombineLayerPath(layerFolder, it.Name));
                }
            }
            App.ViewModel.Toast.Show("导出完成");
        }
        private void ExportWhole(IStorageItem file)
        {
            var fileName = file.Path;
            if (file is StorageFolder)
            {
                fileName = Path.Combine(file.Path, "undefined.png");
            }
            Instance?.SaveAs(fileName);
            App.ViewModel.Toast.Show("导出完成");
        }

    }
}
