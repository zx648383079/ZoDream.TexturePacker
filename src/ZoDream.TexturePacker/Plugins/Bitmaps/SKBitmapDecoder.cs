using SkiaSharp;
using System.IO;
using System.Runtime.InteropServices;
using ZoDream.TexturePacker.ImageEditor;

namespace ZoDream.TexturePacker.Plugins.Bitmaps
{
    public class SKBitmapDecoder : IBitmapDecoder
    {
        public static bool IsSupport(BitmapFormat format)
        {
            return Parse(format) != SKColorType.Unknown;
        }

        public static SKColorType Parse(BitmapFormat format)
        {
            return format switch
            {
                BitmapFormat.A8 => SKColorType.Alpha8,
                BitmapFormat.A16 => SKColorType.Alpha16,
                BitmapFormat.G8 => SKColorType.Gray8,
                BitmapFormat.RG1616 => SKColorType.Rg1616,
                BitmapFormat.RGBX8888 => SKColorType.Rgb888x,
                BitmapFormat.RGB565 => SKColorType.Rgb565,
                BitmapFormat.RGBA1010102 => SKColorType.Rgba1010102,
                BitmapFormat.RGBA16161616 => SKColorType.Rgba16161616,
                BitmapFormat.BGRA8888 => SKColorType.Bgra8888,
                BitmapFormat.RGBA4444 => SKColorType.Argb4444,
                BitmapFormat.RGBA8888 => SKColorType.Rgba8888,
                BitmapFormat.RG88 => SKColorType.Rg88,
                _ => SKColorType.Unknown,
            };
        }

        public static SKBitmap Decode(byte[] data, int width, int height, BitmapFormat format)
        {
            return Decode(data, width, height, Parse(format));
        }
        public static SKBitmap Decode(byte[] buffer, int width, int height, SKColorType format)
        {
            var data = SKData.CreateCopy(buffer);
            var newInfo = new SKImageInfo(width, height, format);
            var bitmap = new SKBitmap();
            bitmap.InstallPixels(newInfo, data.Data);
            return bitmap;
        }

        public SKBitmap Decode(byte[] data)
        {
            return SKBitmap.Decode(data);
        }

        public SKBitmap Decode(Stream input)
        {
            return SKBitmap.Decode(input);
        }

        public SKBitmap Decode(string fileName)
        {
            return SKBitmap.Decode(fileName);
        }

        public byte[] Encode(SKBitmap data)
        {
            var res = data.Encode(SKEncodedImageFormat.Dng, 100);
            return res.AsSpan().ToArray();
        }

        public void Encode(SKBitmap data, Stream output)
        {
            data.Encode(output, SKEncodedImageFormat.Dng, 100);
        }

        public void Encode(SKBitmap data, string fileName)
        {
            data.SaveAs(fileName);
        }
    }
}
