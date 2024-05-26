using SkiaSharp.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Models;
using ZoDream.TexturePacker.Plugins;
using ZoDream.TexturePacker.Plugins.Readers.Unity;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {

        

        public ICommand DragImageCommand { get; private set; }
        public ICommand LayerSelectedCommand { get; private set; }

        private void OnLayerSelected(LayerViewModel? layer)
        {
            if (layer is null)
            {
                return;
            }
            Editor.Select(layer.Id);
        }

        private async void OnDragImage(IReadOnlyList<IStorageItem> items)
        {
            var imageLayer = new Dictionary<string, BitmapImageLayer>();
            var packerMap = new Dictionary<string, IStorageFile>();
            foreach (var item in items)
            {
                if (item is not IStorageFile file)
                {
                    continue;
                }
                var name = Path.GetFileNameWithoutExtension(item.Path);
                if (file.ContentType.Contains("image"))
                {
                    var layer = await Editor.AddImageAsync(file);
                    AddLayer(layer.Id, name, layer.GetPreviewSource());
                    imageLayer.TryAdd(name, layer);
                    continue;
                }
                if (file.ContentType.Contains("json"))
                {
                    packerMap.TryAdd(name, file);
                    continue;
                }
            }
            foreach (var item in packerMap)
            {
                if (!imageLayer.TryGetValue(item.Key, out var layer))
                {
                    continue;
                }
                var data = await new JsonReader().ReadAsync(item.Value);
                if (data is null || data.Items.Count == 0) {
                    continue;
                }
                var parentLayer = GetLayer(layer.Id)!;
                foreach (var kid in data.Items)
                {
                    var kidLayer = layer.Split(kid);
                    Editor.Add(kidLayer);
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
            Editor.Resize();
            Editor.Invalidate();
        }

        
    }
}
