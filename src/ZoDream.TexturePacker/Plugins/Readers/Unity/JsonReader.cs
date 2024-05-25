using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Unity
{
    public class JsonReader : IPluginReader
    {
        public async Task<LayerGroupItem?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return JsonSerializer.Deserialize<UnityLayerGroup>(text);
        }

        public async Task<LayerGroupItem?> ReadAsync(StorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return JsonSerializer.Deserialize<UnityLayerGroup>(text);
        }

        public async Task WriteAsync(string fileName, LayerGroupItem data)
        {
            await LocationStorage.WriteAsync(fileName, JsonSerializer.Serialize(data));
        }

        public async Task WriteAsync(StorageFile file, LayerGroupItem data)
        {
            await FileIO.WriteTextAsync(file, JsonSerializer.Serialize(data), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }

    internal class UnityLayerGroup : LayerGroupItem 
    {
        [JsonPropertyName("imagePath")]
        public new string FileName { get; set; } = string.Empty;

        [JsonPropertyName("SubTexture")]
        public new IList<LayerItem> Items { get; set; } = [];

    }
}
