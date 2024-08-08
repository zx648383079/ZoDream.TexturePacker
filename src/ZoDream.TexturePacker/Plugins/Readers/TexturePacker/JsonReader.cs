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

        public LayerGroupItem? Deserialize(string content)
        {
            var data = JsonSerializer.Deserialize<TP_FrameRoot>(content, _option);
            if (data is null)
            {
                return null;
            }
            return new LayerGroupItem()
            {
                Name = data.Meta.Image,
                FileName = data.Meta.Image,
                Width = data.Meta.Size.W,
                Height = data.Meta.Size.H,
                Items = data.Frames.Select(item => {
                    if (item.Rotated)
                    {
                        return new LayerItem()
                        {
                            Name = item.Filename,
                            X = item.Frame.X,
                            Y = item.Frame.Y,
                            Width = item.Frame.H,
                            Height = item.Frame.W,
                        };
                    }
                    return new LayerItem()
                    {
                        Name = item.Filename,
                        X = item.Frame.X,
                        Y = item.Frame.Y,
                        Width = item.Frame.W,
                        Height = item.Frame.H,
                        // Rotate = item.Rotated ? 90: 0,
                    };
                }).ToArray(),
            };
        }

        public async Task<LayerGroupItem?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text);
        }

        public async Task<LayerGroupItem?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text);
        }

        public string Serialize(LayerGroupItem data)
        {
            // TODO
            return JsonSerializer.Serialize(data, _option);
        }

        public async Task WriteAsync(string fileName, LayerGroupItem data)
        {
            await LocationStorage.WriteAsync(fileName, Serialize(data));
        }

        public async Task WriteAsync(IStorageFile file, LayerGroupItem data)
        {
            await FileIO.WriteTextAsync(file, Serialize(data), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
