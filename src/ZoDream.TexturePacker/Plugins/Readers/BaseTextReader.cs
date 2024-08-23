using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public abstract class BaseTextReader : IPluginReader, ITextReader
    {
        public virtual bool Canable(string content)
        {
            return true;
        }

        public abstract IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName);

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

        public abstract string Serialize(IEnumerable<SpriteLayerSection> data, string fileName);

        public async Task WriteAsync(string fileName, IEnumerable<SpriteLayerSection> data)
        {
            if (!data.Any())
            {
                return;
            }
            await LocationStorage.WriteAsync(fileName, Serialize(data, fileName));
        }

        public async Task WriteAsync(IStorageFile file, IEnumerable<SpriteLayerSection> data)
        {
            if (!data.Any())
            {
                return;
            }
            await FileIO.WriteTextAsync(file, Serialize(data, file.Path), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
