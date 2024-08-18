using SkiaSharp;
using System.IO;

namespace ZoDream.TexturePacker.Plugins.Bitmaps
{
    public abstract class BaseBitmapDecoder : IBitmapDecoder
    {
        public virtual SKBitmap Decode(Stream input)
        {
            var buffer = new byte[input.Length - input.Position];
            input.Read(buffer, 0, buffer.Length);
            return Decode(buffer);
        }

        public abstract SKBitmap Decode(byte[] data);

        public SKBitmap Decode(string fileName)
        {
            using var fs = File.OpenRead(fileName);
            return Decode(fs);
        }

        public abstract byte[] Encode(SKBitmap data);

        public virtual void Encode(SKBitmap data, Stream output)
        {
            output.Write(Encode(data));
        }

        public void Encode(SKBitmap data, string fileName)
        {
            using var fs = File.OpenWrite(fileName);
            Encode(data, fs);
        }
    }
}
