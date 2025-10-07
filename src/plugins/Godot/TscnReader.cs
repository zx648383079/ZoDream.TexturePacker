using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;

namespace ZoDream.Plugin.Godot
{
    public class TscnReader : IPluginReader, ITextReader
    {
        public bool IsEnabled(string content)
        {
            return content.StartsWith("[gd_scene") && content.Contains("uid=\"");
        }

        public IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<ISpriteSection>?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text, fileName);
        }

        public string Serialize(IEnumerable<ISpriteSection> data, string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task WriteAsync(string fileName, IEnumerable<ISpriteSection> data)
        {
            await LocationStorage.WriteAsync(fileName, Serialize(data, fileName));
        }

    }
}
