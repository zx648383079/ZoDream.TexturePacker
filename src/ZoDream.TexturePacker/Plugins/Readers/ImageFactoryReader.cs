using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Drawing;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public class ImageFactoryReader : IImageReader
    {
        public Task<IImageData?> ReadAsync(string fileName)
        {
            return Task.FromResult(new FileImageData(fileName) as IImageData);
        }

        public async Task<IImageData?> ReadAsync(IStorageFile file)
        {
            return await ReadAsync(file.Path);
        }

        public Task WriteAsync(string fileName, IImageData data)
        {
            data.ToBitmap()?.SaveAs(fileName);
            return Task.CompletedTask;
        }

        public Task WriteAsync(IStorageFile file, IImageData data)
        {
            return WriteAsync(file.Path, data);
        }
    }
}
