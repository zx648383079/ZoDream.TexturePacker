using SkiaSharp;
using ZoDream.TexturePacker.Plugins.Bitmaps;

namespace ZoDream.TexturePacker.Plugins
{
    public interface IImageData
    {
        public SKBitmap? TryParse();
    }

    public class ByteImageData(byte[] buffer, int width, int height, SKColorType format): IImageData
    {
        public SKBitmap? TryParse()
        {
            return SKBitmapDecoder.Decode(buffer, width, height, format);
        }
    }

    public class FileByteImageData(byte[] buffer) : IImageData
    {
        public SKBitmap? TryParse()
        {
            return SKBitmap.Decode(buffer);
        }
    }

    public class FileImageData(string fileName) : IImageData
    {
        public SKBitmap? TryParse()
        {
            return new SKBitmapDecoder().Decode(fileName);
        }
    }

    public class ImageImageData(SKBitmap bitmap) : IImageData
    {
        public SKBitmap? TryParse()
        {
            return bitmap.Copy();
        }
    }
}
