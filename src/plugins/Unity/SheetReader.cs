using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Unity
{
    /// <summary>
    /// 单独的区分
    /// </summary>
    public class SheetReader : IPluginReader<IEnumerable<ISpriteSection>>
    {
        public async Task<IEnumerable<ISpriteSection>?> ReadAsync(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            var name = Path.GetFileName(fileName);
            if (name.EndsWith(".json"))
            {
                name = name[..name.LastIndexOf('_')];
            } else
            {
                name = name[..name.IndexOf('.')];
            }
            var res = new SpriteLayerSection()
            {
                Name = name,
                FileName = Path.Combine(folder, $"{name}.png"),
                UseCustomName = true
            };
            foreach (var item in Directory.GetFiles(folder, $"{name}_*.json"))
            {
                var data = JsonSerializer.Deserialize<U3D_SheetDocument>(await File.ReadAllTextAsync(item));
                if (data is null)
                {
                    continue;
                }
                res.Items.Add(new SpriteLayer()
                {
                    Name = data.Name,
                    X = data.Source.Rect.X,
                    Y = data.Source.Rect.Y,
                    Width = data.Source.Rect.Width,
                    Height = data.Source.Rect.Height,
                });
            }
            return [res];
        }

        public Task WriteAsync(string fileName, IEnumerable<ISpriteSection> data)
        {
            throw new NotImplementedException();
        }
    }
}
