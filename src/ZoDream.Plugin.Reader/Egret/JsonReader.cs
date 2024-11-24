using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Readers.Egret
{
    public class JsonReader : BaseTextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowCaseJsonNamingPolicy()
        };
        public override bool IsEnabled(string content)
        {
            return content.Contains("\"frames\"");
        }

        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            var data = JsonSerializer.Deserialize<ER_FrameSheetFile>(content, _option);
            if (data is null)
            {
                return null;
            }
            return [new SpriteLayerSection()
            {
                Name = data.File,
                FileName = data.File,
                Items = data.Frames.Select(item => new SpriteLayer()
                {
                    Name = FormatFileName(item.Key),
                    X = item.Value.X,
                    Y = item.Value.Y,
                    Width = item.Value.W,
                    Height = item.Value.H,
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


        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            // TODO
            return JsonSerializer.Serialize(data, _option);
        }

    }
}
