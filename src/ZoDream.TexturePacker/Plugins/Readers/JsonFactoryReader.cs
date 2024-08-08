using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public class JsonFactoryReader: IPluginReader
    {
        private readonly ITextReader[] Items = [
            new TexturePacker.JsonReader(),
            new Egret.JsonReader(),
            new Unity.JsonReader(),
        ];

        protected virtual ITextReader Default => Items.First() ?? new Unity.JsonReader();

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

        protected virtual LayerGroupItem? Deserialize(string content)
        {
            foreach (var item in Items)
            {
                if (item.Canable(content))
                {
                    return item.Deserialize(content);
                }
            }
            return null;
        }

        public async Task WriteAsync(string fileName, LayerGroupItem data)
        {
            await LocationStorage.WriteAsync(fileName, Default.Serialize(data));
        }

        public async Task WriteAsync(IStorageFile file, LayerGroupItem data)
        {
            await FileIO.WriteTextAsync(file, Default.Serialize(data), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
