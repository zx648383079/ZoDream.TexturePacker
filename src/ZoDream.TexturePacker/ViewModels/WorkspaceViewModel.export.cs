﻿using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Search;
using ZoDream.TexturePacker.Dialogs;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Models;
using ZoDream.TexturePacker.Plugins;
using ZoDream.TexturePacker.Plugins.Readers;
using ZoDream.TexturePacker.Plugins.Readers.Godot;

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
            switch (dialog.ViewModel.SourceIndex)
            {
                case 1:
                    ExportEveryLayer(fileName);
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
            var layer = Editor?.Get<IImageLayer>(SelectedLayer.Id);
            if (layer == null) 
            {
                return;
            }
            var fileName = file.Path;
            if (file is StorageFolder) 
            {
                fileName = Path.Combine(file.Path, SelectedLayer.Name);
            }
            Editor?.SaveAs(layer, fileName);
        }
        private void ExportEveryLayer(IStorageItem file)
        {
            var root = file.Path;
            if (file is IStorageFile)
            {
                root = Path.GetDirectoryName(file.Path);
            }
            foreach (var item in LayerItems)
            {
                if (!item.IsVisible) 
                {
                    continue;
                }
                if (item.Children.Count == 0)
                {
                    Editor?.SaveAs(Editor.Get<IImageLayer>(item.Id), 
                        Path.Combine(root, item.Name));
                    continue;
                }
                foreach (var it in item.Children)
                {
                    if (!it.IsVisible)
                    {
                        continue;
                    }
                    Editor?.SaveAs(Editor.Get<IImageLayer>(it.Id),
                        Path.Combine(root, it.Name));
                }
            }
        }
        private void ExportWhole(IStorageItem file)
        {
            var fileName = file.Path;
            if (file is StorageFolder)
            {
                fileName = Path.Combine(file.Path, "undefined.png");
            }
            Editor?.SaveAs(fileName);
        }

    }
}