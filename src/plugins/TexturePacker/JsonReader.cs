using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.TexturePacker
{
    public class JsonReader : BaseTextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowCaseJsonNamingPolicy()
        };
        public override bool IsEnabled(string content)
        {
            return content.Contains("http://www.codeandweb.com/texturepacker");
        }

        public override IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            var data = JsonSerializer.Deserialize<TP_FrameRoot>(content, _option);
            if (data is null)
            {
                return null;
            }
            return [new SpriteLayerSection()
            {
                Name = data.Meta.Image,
                FileName = data.Meta.Image,
                Width = data.Meta.Size.W,
                Height = data.Meta.Size.H,
                Items = data.Frames.Select(item => {
                    var w = item.Frame.W;
                    var h = item.Frame.H;

                    if (item.Rotated)
                    {
                        (w, h) = (h,  w);
                    }
                    return new SpriteLayer()
                    {
                        Name = item.Filename,
                        X = item.Frame.X,
                        Y = item.Frame.Y,
                        Width = w,
                        Height = h,
                        Rotate = item.Rotated ? 90: 0,
                    };
                }).ToArray(),
            }];
        }


        public override string Serialize(IEnumerable<ISpriteSection> data, string fileName)
        {
            // TODO
            return JsonSerializer.Serialize(data.First(), _option);
        }

    }
}
