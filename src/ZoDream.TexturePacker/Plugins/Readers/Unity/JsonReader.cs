using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Unity
{
    public class JsonReader : IPluginReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new UnityJsonNamingPolicy()
        };

        public async Task<LayerGroupItem?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return JsonSerializer.Deserialize<LayerGroupItem>(text, _option);
        }

        public async Task<LayerGroupItem?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return JsonSerializer.Deserialize<LayerGroupItem>(text, _option);
        }

        public async Task WriteAsync(string fileName, LayerGroupItem data)
        {
            await LocationStorage.WriteAsync(fileName, JsonSerializer.Serialize(data, _option));
        }

        public async Task WriteAsync(IStorageFile file, LayerGroupItem data)
        {
            await FileIO.WriteTextAsync(file, JsonSerializer.Serialize(data, _option), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
