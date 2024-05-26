using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
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
                    imageLayer.TryAdd(name, await Editor.AddImageAsync(file));
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
                var newLayers = layer.Split(data.Items);
                Editor.Add(newLayers);
                AddLayer(data);
            }
            // var (width, height) = new CssSprites().Compute([.. Editor.LayerItems]);
            // Editor.Resize(width, height);
            Editor.Resize();
            Editor.Invalidate();
        }
    }
}
