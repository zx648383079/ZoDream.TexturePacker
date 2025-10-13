using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
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
            var sourceIndex = dialog.ViewModel.SourceIndex;
            var typeIndex = dialog.ViewModel.TypeIndex;
            switch (sourceIndex)
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
            if (sourceIndex > 0 || typeIndex < 1 || fileName is not IStorageFile target)
            {
                return;
            }
            var writer = dialog.ViewModel.CreateWriter(target);
            if (writer is null)
            {
                return;
            }
            await writer.WriteAsync(target.Path, [Serialize(Path.GetFileNameWithoutExtension(fileName.Name) + ".png")]);
        }

        private ISpriteSection Serialize(string fileName)
        {
            var res = new SpriteLayerSection()
            {
                FileName = fileName,
                Width = Instance.ActualWidthI,
                Height = Instance.ActualHeightI,
            };
            foreach (var item in Instance.LayerItems)
            {
                if (string.IsNullOrEmpty(res.Name))
                {
                    res.Name = item.Name;
                }
                foreach (var it in item.Children)
                {
                    if (!it.IsVisible)
                    {
                        continue;
                    }
                    res.Items.Add(new SpriteLayer()
                    {
                        Name = it.Name,
                        X = it.Source.X,
                        Y = it.Source.Y,
                        Width = it.Source.Width,
                        Height = it.Source.Height,
                        Rotate = it.Source.Rotate,
                    });
                }
            }
            return res;
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
            if (!fileName.EndsWith(".png"))
            {
                var i = fileName.LastIndexOf('.');
                fileName = fileName[..i] + ".png";
            }
            Instance?.SaveAs(fileName);
            App.ViewModel.Toast.Show("导出完成");
        }

    }
}
