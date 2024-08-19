using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Gimp
{
    public class XcfReader : IProjectReader
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
