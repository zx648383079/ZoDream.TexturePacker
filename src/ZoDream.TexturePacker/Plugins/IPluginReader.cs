﻿using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins
{
    public interface IPluginReader
    {
        public Task<LayerGroupItem?> ReadAsync(string fileName);
        public Task<LayerGroupItem?> ReadAsync(StorageFile file);

        public Task WriteAsync(string fileName, LayerGroupItem data);
        public Task WriteAsync(StorageFile file, LayerGroupItem data);
    }
}
