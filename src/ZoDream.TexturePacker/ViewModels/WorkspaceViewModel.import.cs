using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using ZoDream.TexturePacker.Dialogs;
using ZoDream.TexturePacker.ImageEditor;
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
                if (layer is null)
                {
                    continue;
                }
                AddLayer(layer.Id, name, layer.GetPreviewSource());
            }
            foreach (var file in partFiles)
            {
                var name = Path.GetFileNameWithoutExtension(file.Path);
                var data = await ReaderFactory.LoadLayerAsync(file);
                if (file.FileType == ".tres")
                {
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
