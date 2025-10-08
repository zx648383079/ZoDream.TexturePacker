using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.TexturePacker
{
    public class JsonHashReader : BaseTextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowCaseJsonNamingPolicy()
        };
        public override bool IsEnabled(string content)
        {
            return content.Contains("\"frames\"") && content.Contains("frame") && content.Contains("sourceSize");
        }

        public override IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            var data = JsonSerializer.Deserialize<TP_FrameRoot2>(content, _option);
            if (data is null)
            {
                return null;
            }
            var name = data.Meta.Image;
            return [new SpriteLayerSection()
            {
                Name = name,
                FileName = name,
                Items = data.Frames.Select(item => new SpriteLayer()
                {
                    Name = FormatFileName(item.Key),
                    X = item.Value.Frame.X,
                    Y = item.Value.Frame.Y,
                    Width = item.Value.Frame.W,
                    Height = item.Value.Frame.H,
                }).ToArray(),
            }];
        }

        private string FormatFileName(string val)
        {
            var i = val.LastIndexOf('_');
            if (i > 0)
            {
                return val[0..i] + '.' + val[(i + 1)..];
            }
            return val;
        }


        public override string Serialize(IEnumerable<ISpriteSection> data, string fileName)
        {
            // TODO
            return JsonSerializer.Serialize(data, _option);
        }
    }
}
