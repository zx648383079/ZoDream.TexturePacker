using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.IO;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Godot
{
    public class TscnReader : IPluginReader, ITextReader
    {
        public bool IsEnabled(string content)
        {
            return content.StartsWith("[gd_scene") && content.Contains("uid=\"");
        }

        public IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            throw new NotImplementedException();
        }


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

        public string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task WriteAsync(string fileName, IEnumerable<SpriteLayerSection> data)
        {
            await LocationStorage.WriteAsync(fileName, Serialize(data, fileName));
        }

        public async Task WriteAsync(IStorageFile file, IEnumerable<SpriteLayerSection> data)
        {
            await FileIO.WriteTextAsync(file, Serialize(data, file.Path), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
