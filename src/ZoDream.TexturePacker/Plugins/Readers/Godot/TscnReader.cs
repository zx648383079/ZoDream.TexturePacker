using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Godot
{
    public class TscnReader : IPluginReader
    {
        public Task<LayerGroupItem?> ReadAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<LayerGroupItem?> ReadAsync(StorageFile file)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(string fileName, LayerGroupItem data)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(StorageFile file, LayerGroupItem data)
        {
            throw new NotImplementedException();
        }
    }
}
