﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.TexturePacker
{
    public class JsonReader : BaseTextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowcaseJsonNamingPolicy()
        };
        public override bool Canable(string content)
        {
            return content.Contains("http://www.codeandweb.com/texturepacker");
        }

        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
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


        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            // TODO
            return JsonSerializer.Serialize(data.First(), _option);
        }

    }
}
