using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Live2d
{
    public partial class MocReader : IPluginReader
    {
        public Task<IEnumerable<SpriteLayerSection>?> ReadAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                var jsonFileName = GetModelJsonFile(fileName);
                if (jsonFileName is null)
                {
                    return null;
                }
                return Read(fileName, MocJsonReader.LoadTexture(jsonFileName));
            });
        }


        private string GetModelJsonFile(string fileName)
        {
            var baseFile = fileName.Substring(0, fileName.Length -
                Path.GetExtension(fileName).Length);
            fileName = baseFile + ".model3.json";
            if (File.Exists(fileName))
            {
                return fileName;
            }
            fileName = baseFile + ".model.json";
            if (File.Exists(fileName))
            {
                return fileName;
            }
            fileName = baseFile + ".json";
            return File.Exists(fileName) ? fileName : string.Empty;
        }

        public static IEnumerable<SpriteLayerSection>? Read(string fileName, string[] textureItems)
        {
            if (textureItems.Length == 0 || !File.Exists(fileName))
            {
                return null;
            }
            using var fs = File.OpenRead(fileName);
            return Read(fs, textureItems);
        }

        public Task WriteAsync(string fileName, IEnumerable<SpriteLayerSection> data)
        {
            throw new NotImplementedException();
        }

    }
}
