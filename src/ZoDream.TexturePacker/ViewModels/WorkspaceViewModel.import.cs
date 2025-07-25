﻿using SkiaSharp;
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
using ZoDream.Shared.Interfaces;
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
            IsLoading = true;
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
            IsLoading = false;
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
            IsLoading = true;
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
            IsLoading = false;
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
                OnDragImage(new FileLoader(folder));
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
            OnDragImage(new FileLoader(items));
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
            OnDragImage(new FileLoader(items));
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
        private void OnDragImage(IEnumerable<IStorageItem> items)
        {
            OnDragImage(new FileLoader(items));
        }
        private async void OnDragImage(FileLoader loader)
        {
            IsLoading = true;
            if (!await loader.LoadAsync())
            {
                IsLoading = false;
                return;
            }
            await foreach (var item in loader.EnumerateImage())
            {
                var layer = Instance!.AddImage(item.Source);
                if (layer is not null)
                {
                    layer.Name = Path.GetFileNameWithoutExtension(item.FileName);
                    AddLink(layer.Id, item.MetaItems);
                    AddLink(layer.Id, Path.GetFileName(item.FileName), layer.Name);
                }
            }
            await foreach (var item in loader.EnumerateLayer())
            {
                await ImportSpriteAsync(item);
            }
            await foreach (var item in loader.EnumerateSkeleton())
            {
                AddSkin(item.Skins);
                AddSlot(item.Slots);
                AddAnimation(item.Animations);
                if (item is ISkeletonController ctl)
                {
                    _styleManager.Add(new SkeletonImageStyler("ctl", ctl));
                } else if (item is SkeletonSection s)
                {
                    _styleManager.Add(new SkeletonImageStyler(s));
                }
            }
            AddResource(loader.ResourceItems);
            Instance!.Resize();
            Instance.Invalidate();
            IsLoading = false;
        }

        private async Task ImportSpriteAsync(SpriteLayerSection data)
        {
            if (data.Items.Count == 0)
            {
                return;
            }
            var parentLayer = GetLayerWithLink(data.Name!) ?? await AddImageAsync(data.FileName);
            if (parentLayer is null)
            {
                return;
            }
            if (parentLayer.Source is not BitmapImageSource layerImage)
            {
                return;
            }
            foreach (var kid in data.Items)
            {
                var kidLayer = layerImage.Split(kid);
                if (kidLayer is null)
                {
                    continue;
                }
                Instance!.Add(Create(kidLayer, kid.Name), parentLayer);
            }
            parentLayer.IsVisible = false;
        }
        
    }
}
