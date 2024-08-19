using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.TexturePacker
{
    public class JsonReader : IPluginReader, ITextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowcaseJsonNamingPolicy()
        };
        public bool Canable(string content)
        {
            return content.Contains("http://www.codeandweb.com/texturepacker");
        }

        public SpriteLayerSection? Deserialize(string content)
        {
            var data = JsonSerializer.Deserialize<TP_FrameRoot>(content, _option);
            if (data is null)
            {
                return null;
            }
            return new SpriteLayerSection()
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
            };
        }

        public async Task<SpriteLayerSection?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text);
        }

        public async Task<SpriteLayerSection?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text);
        }

        public string Serialize(SpriteLayerSection data)
        {
            // TODO
            return JsonSerializer.Serialize(data, _option);
        }

        public async Task WriteAsync(string fileName, SpriteLayerSection data)
        {
            await LocationStorage.WriteAsync(fileName, Serialize(data));
        }

        public async Task WriteAsync(IStorageFile file, SpriteLayerSection data)
        {
            await FileIO.WriteTextAsync(file, Serialize(data), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
