﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Live2d
{
    public class MocJsonReader : BaseTextReader
    {
        public override bool Canable(string content)
        {
            return content.Contains("\"FileReferences\"");
        }

        public static string[] LoadTexture(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            var data = JsonSerializer.Deserialize<LD_ModelRoot>(File.ReadAllText(fileName));
            if (data is null)
            {
                return [];
            }
            return data.FileReferences.Textures.Select(i => Path.Combine(folder, i)).ToArray();
        }

        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            var data = JsonSerializer.Deserialize<LD_ModelRoot>(content);
            if (data is null) 
            {
                return null;
            }
            return MocReader.Read(Path.Combine(folder, data.FileReferences.Moc),
                data.FileReferences.Textures.Select(i => Path.Combine(folder, i)).ToArray());
        }

        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
