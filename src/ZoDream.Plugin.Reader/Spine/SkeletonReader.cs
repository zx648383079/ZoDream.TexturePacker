using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Readers.Spine
{
    /// <summary>
    /// skel 文件读取
    /// </summary>
    public partial class SkeletonReader : ISkeletonReader
    {
        public Task<IEnumerable<SkeletonSection>?> ReadAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                using var fs = File.OpenRead(fileName);
                return Read(fs);
            });
        }

        public Task WriteAsync(string fileName, IEnumerable<SkeletonSection> data)
        {
            throw new NotImplementedException();
        }

    }
}
