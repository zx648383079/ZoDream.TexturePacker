using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Spine
{
    /// <summary>
    /// skel 文件读取
    /// </summary>
    public partial class SkeletonReader : ISkeletonReader
    {
        public Task<IEnumerable<ISkeleton>?> ReadAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                using var fs = File.OpenRead(fileName);
                return Read(fs);
            });
        }

        public Task WriteAsync(string fileName, IEnumerable<ISkeleton> data)
        {
            throw new NotImplementedException();
        }

    }
}
