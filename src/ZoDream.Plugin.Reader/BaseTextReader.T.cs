using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;

namespace ZoDream.Plugin.Readers
{
    public abstract class BaseTextReader<T> : IPluginReader<IEnumerable<T>>, ITextReader<IEnumerable<T>>

    {
        public virtual bool IsEnabled(string content)
        {
            return true;
        }

        protected virtual string GetFullPath(string fileName)
        {
            return fileName;
        }

        public abstract IEnumerable<T>? Deserialize(string content, string fileName);



        public async Task<IEnumerable<T>?> ReadAsync(string fileName)
        {
            fileName = GetFullPath(fileName);
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text, fileName);
        }

        public abstract string Serialize(IEnumerable<T> data, string fileName);

        public async Task WriteAsync(string fileName, IEnumerable<T> data)
        {
            if (!data.Any())
            {
                return;
            }
            fileName = GetFullPath(fileName);
            await LocationStorage.WriteAsync(fileName, Serialize(data, fileName));
        }
    }
}
