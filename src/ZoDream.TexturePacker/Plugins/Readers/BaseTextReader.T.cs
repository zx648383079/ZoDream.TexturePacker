using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public abstract class BaseTextReader<T> : IPluginReader<IEnumerable<T>>, ITextReader<IEnumerable<T>>

    {
        public virtual bool Canable(string content)
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

        public async Task<IEnumerable<T>?> ReadAsync(IStorageFile file)
        {
            return await ReadAsync(file.Path);
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

        public async Task WriteAsync(IStorageFile file, IEnumerable<T> data)
        {
            await WriteAsync(file.Path, data);
        }
    }
}
