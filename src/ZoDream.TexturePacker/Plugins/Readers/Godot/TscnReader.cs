using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Godot
{
    public class TscnReader : IPluginReader, ITextReader
    {
        public bool Canable(string content)
        {
            return content.StartsWith("[gd_scene") && content.Contains("uid=\"");
        }

        public LayerGroupItem? Deserialize(string content)
        {
            throw new NotImplementedException();
        }


        public async Task<LayerGroupItem?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text);
        }

        public async Task<LayerGroupItem?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text);
        }

        public string Serialize(LayerGroupItem data)
        {
            throw new NotImplementedException();
        }

        public async Task WriteAsync(string fileName, LayerGroupItem data)
        {
            await LocationStorage.WriteAsync(fileName, Serialize(data));
        }

        public async Task WriteAsync(IStorageFile file, LayerGroupItem data)
        {
            await FileIO.WriteTextAsync(file, Serialize(data), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
