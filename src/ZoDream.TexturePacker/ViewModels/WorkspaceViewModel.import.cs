using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
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

        private async void TapImportFolder(object? _)
        {
            var picker = new FolderPicker();
            App.ViewModel.InitializePicker(picker);
            var folder = await picker.PickSingleFolderAsync();
            if (folder != null) 
            {
                OnDragImage([folder]);
            }
        }

        private async void TapImportFile(object? _)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".tres");
            picker.FileTypeFilter.Add("*");
            App.ViewModel.InitializePicker(picker);
            var items = await picker.PickMultipleFilesAsync();
            OnDragImage(items);
        }

        private async void TapImport(object? _)
        {
            var dialog = new ImportDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }

        private async void OnDragImage(IEnumerable<IStorageItem> items)
        {
            var (imageFiles, partFiles) = await LoadFiles(items);
            if (imageFiles.Length == 0 && partFiles.Length == 0)
            {
                return;
            }
            foreach (var file in imageFiles)
            {
                var name = Path.GetFileNameWithoutExtension(file.Path);
                var layer = await Editor!.AddImageAsync(file);
                AddLayer(layer.Id, name, layer.GetPreviewSource());
            }
            foreach (var file in partFiles)
            {
                LayerGroupItem? data = null;
                var name = Path.GetFileNameWithoutExtension(file.Path);
                if (file.ContentType.Contains("json"))
                {
                    data = await new JsonFactoryReader().ReadAsync(file);
                } else if (file.FileType == ".tres")
                {
                    data = await new TresReader().ReadAsync(file);
                    name = data?.Name;
                }
                if (data is null || data.Items.Count == 0)
                {
                    continue;
                }
                var parentLayer = GetLayer(name!);
                if (parentLayer is null)
                {
                    continue;
                }
                var layer = Editor!.Get<BitmapImageLayer>(parentLayer.Id);
                if (layer is null)
                {
                    continue;
                }
                foreach (var kid in data.Items)
                {
                    var kidLayer = layer.Split(kid);
                    Editor!.Add(kidLayer);
                    parentLayer.Children.Add(new LayerViewModel()
                    {
                        Id = kidLayer.Id,
                        Name = kid.Name,
                        PreviewImage = kidLayer.GetPreviewSource()
                    });
                }
                layer.Visible = false;
            }
            // var (width, height) = new CssSprites().Compute([.. Editor.LayerItems]);
            // Editor.Resize(width, height);
            Editor!.Resize();
            Editor.Invalidate();
        }


        private static async Task<(IStorageFile[], IStorageFile[])> LoadFiles(IEnumerable<IStorageItem> items)
        {
            var images = new List<IStorageFile>();
            var partItems = new List<IStorageFile>();
            await EachFile(items, file => {
                if (file.ContentType.Contains("image"))
                {
                    images.Add(file);
                    return;
                }
                if (file.ContentType.Contains("json") || file.FileType == ".tres")
                {
                    partItems.Add(file);
                }
            });
            return (images.ToArray(), partItems.ToArray());
        }

        private static async Task EachFile(IEnumerable<IStorageItem> items, Action<IStorageFile> cb)
        {
            foreach (var item in items)
            {
                if (item is IStorageFile file)
                {
                    cb(file);
                    continue;
                }
                if (item is IStorageFolder folder)
                {
                    await EachFile(await folder.GetItemsAsync(), cb);
                }
            }
        }

    }
}
