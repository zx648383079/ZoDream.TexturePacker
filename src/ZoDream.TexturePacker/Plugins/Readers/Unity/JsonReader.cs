﻿using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Unity
{
    public class JsonReader : IPluginReader, ITextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new UnityJsonNamingPolicy()
        };

        public bool Canable(string content)
        {
            return content.Contains("\"items\"");
        }

        public LayerGroupItem? Deserialize(string content)
        {
            return JsonSerializer.Deserialize<LayerGroupItem>(content, _option);
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
