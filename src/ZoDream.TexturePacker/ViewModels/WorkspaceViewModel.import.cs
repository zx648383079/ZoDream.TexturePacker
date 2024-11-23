using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ImageEditor;
using ZoDream.Shared.Models;
using ZoDream.TexturePacker.Dialogs;
using ZoDream.TexturePacker.Plugins;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        /// <summary>
        /// 分离图片
        /// </summary>
        /// <param name="layer"></param>
        private async void SeparateImage(IImageLayer layer)
        {
            if (layer is null || layer.Source is not BitmapImageSource image)
            {
                return;
            }
            var trace = new ImageContourTrace(true);
            var items = await trace.GetContourAsync(image.Source);
            Instance!.Add(items.Select(path => {
                var bound = path.Bounds;
                var kid = image.Source.Clip(path);
                if (kid is null)
                {
                    return null;
                }
                var kidLayer = new BitmapImageSource(
                    kid
                    , Instance)
                {
                    X = (int)bound.Left,
                    Y = (int)bound.Top
                };
                return Create(kidLayer);
            }), layer);
            layer.IsVisible = false;
            Instance.Invalidate();
        }
        /// <summary>
        /// 分离图片对象并重新采样合并到子对象
        /// </summary>
        /// <param name="layer"></param>
        private async void SeparateImageAndMerge(IImageLayer layer)
        {
            if (layer is null || layer.Source is not BitmapImageSource image)
            {
                return;
            }
            var trace = new ImageContourTrace(true);
            var items = await trace.GetContourAsync(image.Source);
            using var paint = new SKPaint();
            foreach (var item in layer.Children)
            {
                if (item.Source is not BitmapImageSource i)
                {
                    continue;
                }
                var path = GetContainPath(i, items);
                if (path is null)
                {
                    continue;
                }
                using var canvas = new SKCanvas(i.Source);
                var rect = path.Bounds;
                canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(image.Source, rect, 
                    SKRect.Create(rect.Left - i.X, rect.Top - i.Y, rect.Width, rect.Height), paint);
                path.Offset(-i.X, -i.Y);
                canvas.ClipPath(path, SKClipOperation.Difference);
                canvas.Clear();
                item.Resample();
            }
            Instance?.Invalidate();
        }

        private static SKPath? GetContainPath(IImageBound image, IEnumerable<SKPath> items)
        {
            var maxDiff = 6;
            var doubleMaxDiff = 2 * maxDiff;
            var minDiff = 2;
            var doubleMinDiff = 2 * minDiff;
            foreach (var item in items)
            {
                var bound = item.Bounds;
                if (image.X > bound.Left + minDiff || image.Y > bound.Top + minDiff
                    || image.Width < bound.Width - doubleMinDiff 
                    || image.Height < bound.Height - doubleMinDiff)
                {
                    continue;
                }
                if (bound.Left - image.X >= maxDiff 
                    || bound.Top - image.Y >= maxDiff
                    || image.Width - bound.Width >= doubleMaxDiff
                    || image.Height - bound.Height >= doubleMaxDiff)
                {
                    continue;
                }
                return item;
            }
            return null;
        }

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

        private async Task<IImageLayer?> AddImageAsync(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return null;
            }
            var layer = await Instance!.AddImageAsync(fileName);
            if (layer is null)
            {
                return null;
            }
            _ = LoadImageMetaAsync(fileName, layer.Id);
            return layer;
        }

        public void DragFileAsync(IEnumerable<IStorageItem> items)
        {
            OnDragImage(items);
        }

        private void AddImage(IImageData data)
        {
            var layer = Instance?.AddImage(data);
            if (layer is null)
            {
                return;
            }
            Instance?.Invalidate();
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
                await ImportSpriteAsync(data, name);
            }
            // var (width, height) = new CssSprites().Compute([.. Editor.LayerItems]);
            // Editor.Resize(width, height);
            Instance!.Resize();
            Instance.Invalidate();
        }

        private async Task ImportSpriteAsync(IEnumerable<SpriteLayerSection>? items, 
            string layerName)
        {
            if (items == null)
            {
                return;
            }
            foreach (var data in items)
            {
                var name = data.UseCustomName ? data.Name : layerName;
                if (data is null || data.Items.Count == 0)
                {
                    continue;
                }
                var parentLayer = GetLayerWithLink(name!) ?? await AddImageAsync(data.FileName);
                if (parentLayer is null)
                {
                    continue;
                }
                if (parentLayer.Source is not BitmapImageSource layerImage)
                {
                    continue;
                }
                foreach (var kid in data.Items)
                {
                    var kidLayer = layerImage.Split(kid);
                    if (kidLayer is null)
                    {
                        continue;
                    }
                    Instance!.Add(Create(kidLayer, kid.Name), parentLayer);;
                }
                parentLayer.IsVisible = false;
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
