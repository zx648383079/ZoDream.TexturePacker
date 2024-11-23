using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Readers.Godot
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

        public string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task WriteAsync(string fileName, IEnumerable<SpriteLayerSection> data)
        {
            await LocationStorage.WriteAsync(fileName, Serialize(data, fileName));
        }

    }
}
