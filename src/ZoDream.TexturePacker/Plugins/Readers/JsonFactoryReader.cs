using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public class JsonFactoryReader: IPluginReader
    {
        private readonly ITextReader[] Items = [
            new Live2d.MocJsonReader(),
            new TexturePacker.JsonReader(),
            new Egret.JsonReader(),
            new Unity.JsonReader(),
        ];

        protected virtual ITextReader Default => Items.First() ?? new Unity.JsonReader();

        public async Task<IEnumerable<SpriteLayerSection>?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text, fileName);
        }

        public async Task<IEnumerable<SpriteLayerSection>?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text, file.Path);
        }

        protected virtual IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            foreach (var item in Items)
            {
                if (item.Canable(content))
                {
                    return item.Deserialize(content, fileName);
                }
            }
            return null;
        }

        public async Task WriteAsync(string fileName, IEnumerable<SpriteLayerSection> data)
        {
            await LocationStorage.WriteAsync(fileName, Default.Serialize(data, fileName));
        }

        public async Task WriteAsync(IStorageFile file, IEnumerable<SpriteLayerSection> data)
        {
            await FileIO.WriteTextAsync(file, Default.Serialize(data, file.Path), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
