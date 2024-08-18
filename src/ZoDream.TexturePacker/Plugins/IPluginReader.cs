using System.Threading.Tasks;
using Windows.Storage;

namespace ZoDream.TexturePacker.Plugins
{
    public interface IPluginReader<T>
    {

        public Task<T?> ReadAsync(string fileName);
        public Task<T?> ReadAsync(IStorageFile file);

        public Task WriteAsync(string fileName, T data);
        public Task WriteAsync(IStorageFile file, T data);
    }
}
