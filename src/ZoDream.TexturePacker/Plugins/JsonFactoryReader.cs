using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;

namespace ZoDream.TexturePacker.Plugins
{
    public class JsonFactoryReader: IPluginReader
    {
        private readonly ITextReader[] Items = [
            new Plugin.Live2d.MocJsonReader(),
            new Plugin.TexturePacker.JsonReader(),
            new Plugin.TexturePacker.JsonHashReader(),
            new Plugin.Egret.JsonReader(),
            new Plugin.Unity.JsonReader(),
        ];

        protected virtual ITextReader Default => Items.First() ?? new Plugin.Unity.JsonReader();

        public async Task<IEnumerable<ISpriteSection>?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text, fileName);
        }

        public async Task<IEnumerable<ISpriteSection>?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text, file.Path);
        }

        protected virtual IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            foreach (var item in Items)
            {
                if (item.IsEnabled(content))
                {
                    return item.Deserialize(content, fileName);
                }
            }
            return null;
        }

        public async Task WriteAsync(string fileName, IEnumerable<ISpriteSection> data)
        {
            await LocationStorage.WriteAsync(fileName, Default.Serialize(data, fileName));
        }

        public async Task WriteAsync(IStorageFile file, IEnumerable<ISpriteSection> data)
        {
            await FileIO.WriteTextAsync(file, Default.Serialize(data, file.Path), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
