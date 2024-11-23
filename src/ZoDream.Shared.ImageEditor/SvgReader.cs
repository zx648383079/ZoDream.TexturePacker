using SkiaSharp;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.ImageEditor
{
    public class SvgReader : IImageReader
    {
        public Task<IImageData?> ReadAsync(string fileName)
        {
            return Task.FromResult(new SvgFileImageData(fileName) as IImageData);
        }


        public Task WriteAsync(string fileName, IImageData data)
        {
            using var stream = File.OpenWrite(fileName);
            using var canvas = SKSvgCanvas.Create(new SKRect(0, 0, 100, 100), stream);
            canvas.DrawBitmap(data.ToBitmap(), 0, 0);
            canvas.Flush();
            return Task.CompletedTask;
        }

    }
}
