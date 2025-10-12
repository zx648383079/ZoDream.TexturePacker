using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ZoDream.Plugin.Live2d.Models;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;

namespace ZoDream.Plugin.Live2d
{
    public class MocJsonReader : BaseTextReader
    {
        public override bool IsEnabled(string content)
        {
            return content.Contains("\"FileReferences\"") && content.Contains("\"Moc\"");
        }

        public static string[] LoadTexture(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            var data = JsonSerializer.Deserialize<JsonModelRoot>(File.ReadAllText(fileName));
            if (data is null)
            {
                return [];
            }
            return data.FileReferences.Textures.Select(i => Path.Combine(folder, i)).ToArray();
        }

        public override IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            var data = JsonSerializer.Deserialize<JsonModelRoot>(content);
            if (data is null) 
            {
                return null;
            }
            return MocReader.Read(Path.Combine(folder, data.FileReferences.Moc),
                data.FileReferences.Textures.Select(i => Path.Combine(folder, i)).ToArray());
        }

        public override string Serialize(IEnumerable<ISpriteSection> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
