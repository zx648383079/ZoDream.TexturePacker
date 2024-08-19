using System;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Adobe
{
    public class PsdReader : IProjectReader
    {
        public Task<ProjectDocument?> ReadAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDocument?> ReadAsync(IStorageFile file)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(string fileName, ProjectDocument data)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(IStorageFile file, ProjectDocument data)
        {
            throw new NotImplementedException();
        }
    }
}
