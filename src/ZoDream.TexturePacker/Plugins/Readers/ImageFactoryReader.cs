﻿using SkiaSharp;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.ImageEditor;

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
            data.TryParse()?.SaveAs(fileName);
            return Task.CompletedTask;
        }

        public Task WriteAsync(IStorageFile file, IImageData data)
        {
            return WriteAsync(file.Path, data);
        }
    }
}
