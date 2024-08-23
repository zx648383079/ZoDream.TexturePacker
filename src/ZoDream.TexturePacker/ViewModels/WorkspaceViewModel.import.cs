using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using ZoDream.TexturePacker.Dialogs;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Models;
using ZoDream.TexturePacker.Plugins;

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
            foreach (var ext in ReaderFactory.FileFilterItems)
            {
                picker.FileTypeFilter.Add(ext);
            }
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

        private async Task<LayerViewModel?> AddImageAsync(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return null;
            }
            var name = Path.GetFileNameWithoutExtension(fileName);
            var layer = await Editor!.AddImageAsync(fileName);
            if (layer is null)
            {
                return null;
            }
            return AddLayer(layer.Id, name, layer.GetPreviewSource());
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
                await AddImageAsync(file.Path);
            }
            foreach (var file in partFiles)
            {
                var name = Path.GetFileNameWithoutExtension(file.Path);
                var data = await ReaderFactory.LoadSpriteAsync(file);
                await ImportSpriteAsync(data, file.FileType == ".tres" ? string.Empty : name);
            }
            // var (width, height) = new CssSprites().Compute([.. Editor.LayerItems]);
            // Editor.Resize(width, height);
            Editor!.Resize();
            Editor.Invalidate();
        }

        private async Task ImportSpriteAsync(IEnumerable<SpriteLayerSection>? items, 
            string layerName)
        {
            if (items == null)
            {
                return;
            }
            BitmapImageLayer? layer;
            foreach (var data in items)
            {
                var name = string.IsNullOrWhiteSpace(layerName) ? data.Name : layerName;
                if (data is null || data.Items.Count == 0)
                {
                    continue;
                }
                var parentLayer = GetLayer(name!) ?? await AddImageAsync(data.FileName);
                if (parentLayer is null)
                {
                    continue;
                }
                layer = Editor!.Get<BitmapImageLayer>(parentLayer.Id);
                if (layer is null)
                {
                    continue;
                }
                foreach (var kid in data.Items)
                {
                    var kidLayer = layer.Split(kid);
                    if (kidLayer is null)
                    {
                        continue;
                    }
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
        }


        private static async Task<(IStorageFile[], IStorageFile[])> LoadFiles(IEnumerable<IStorageItem> items)
        {
            var images = new List<IStorageFile>();
            var partItems = new List<IStorageFile>();
            await EachFile(items, file => {
                if (ReaderFactory.IsImageFile(file))
                {
                    images.Add(file);
                    return;
                }
                if (ReaderFactory.IsLayerFile(file))
                {
                    partItems.Add(file);
                }
            });
            return ([.. images], [.. partItems]);
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
