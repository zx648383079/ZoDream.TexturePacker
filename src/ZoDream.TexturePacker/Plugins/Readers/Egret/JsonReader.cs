﻿using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Egret
{
    public class JsonReader : IPluginReader, ITextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowcaseJsonNamingPolicy()
        };
        public bool Canable(string content)
        {
            return content.Contains("\"frames\"");
        }

        public LayerGroupItem? Deserialize(string content)
        {
            var data = JsonSerializer.Deserialize<ER_FrameSheetFile>(content, _option);
            if (data is null)
            {
                return null;
            }
            return new LayerGroupItem()
            {
                Name = data.File,
                FileName = data.File,
                Items = data.Frames.Select(item => new LayerItem()
                {
                    Name = item.Key,
                    X = item.Value.X,
                    Y = item.Value.Y,
                    Width = item.Value.W,
                    Height = item.Value.H,
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
