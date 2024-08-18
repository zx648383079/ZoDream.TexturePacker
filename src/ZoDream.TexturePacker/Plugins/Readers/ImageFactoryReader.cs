using SkiaSharp;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public class ImageFactoryReader : IImageReader
    {
        public Task<SKBitmap?> ReadAsync(string fileName)
        {
            return Task.FromResult(SKBitmap.Decode(fileName));
        }

        public async Task<SKBitmap?> ReadAsync(IStorageFile file)
        {
            var fs = await file.OpenStreamForReadAsync();
            return SKBitmap.Decode(fs);
        }

        public Task WriteAsync(string fileName, SKBitmap data)
        {
            data.SaveAs(fileName);
            return Task.CompletedTask;
        }

        public Task WriteAsync(IStorageFile file, SKBitmap data)
        {
            data.SaveAs(file.Path);
            return Task.CompletedTask;
        }
    }
}
