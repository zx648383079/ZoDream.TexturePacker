using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Plugins;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private readonly Dictionary<string, int> ImageNameLinkItems = [];


        public async Task LoadImageMetaAsync(string fileName, int layerId)
        {
            var items = await ReaderFactory.LoadImageMetaAsync(fileName);
            foreach (var item in items)
            {
                ImageNameLinkItems.TryAdd(item, layerId);
            }
        }

        public IImageLayer? GetLayerWithLink(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (ImageNameLinkItems.TryGetValue(name, out var item)) 
            {
                return GetLayer(item);
            }
            return GetLayer(name);
        }
    }
}
