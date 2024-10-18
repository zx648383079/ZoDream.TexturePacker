using SkiaSharp;
using Svg.Skia;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Drawing;
using ZoDream.TexturePacker.Drawing;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public class SvgReader : IImageReader
    {
        public Task<IImageData?> ReadAsync(string fileName)
        {
            return Task.FromResult(new SvgFileImageData(fileName) as IImageData);
        }

        public Task<IImageData?> ReadAsync(IStorageFile file)
        {
            return ReadAsync(file.Path);
        }

        public Task WriteAsync(string fileName, IImageData data)
        {
            using var stream = File.OpenWrite(fileName);
            using var canvas = SKSvgCanvas.Create(new SKRect(0, 0, 100, 100), stream);
            canvas.DrawBitmap(data.ToBitmap(), 0, 0);
            canvas.Flush();
            return Task.CompletedTask;
        }

        public Task WriteAsync(IStorageFile file, IImageData data)
        {
            return WriteAsync(file.Path, data);
        }
    }
}
