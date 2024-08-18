using SkiaSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Models;
using ZoDream.TexturePacker.Plugins.Readers;
using ZoDream.TexturePacker.Plugins.Readers.Godot;
using ZoDream.TexturePacker.Plugins.Readers.TexturePacker;

namespace ZoDream.TexturePacker.Plugins
{
    public static class ReaderFactory
    {

        private static string[] ImageFilterItems = [".png", ".jpg", ".jpeg", ".webp", ".pvr", ".ccz"];

        private static string[] LayerFilterItems = [".json", ".tres", ".plist" ];
        public static string[] FileFilterItems = [..ImageFilterItems, ..LayerFilterItems];

        public static bool IsImageFile(string fileName)
        {
            return ImageFilterItems.Contains(Path.GetExtension(fileName));
        }
        public static bool IsImageFile(IStorageFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }
            if (ImageFilterItems.Contains(file.FileType))
            {
                return true;
            }
            return false;
        }

        public static bool IsLayerFile(IStorageFile file)
        {
            if (file.ContentType.Contains("json"))
            {
                return true;
            }
            return LayerFilterItems.Contains(file.FileType);
        }

        public static bool IsLayerFile(string fileName)
        {
            return LayerFilterItems.Contains(Path.GetExtension(fileName));
        }

        public static IImageReader? GetImageReader(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension == ".pvr" || extension == ".ccz")
            {
                return new PvrReader();
            }
            return IsImageFile(fileName) ? new ImageFactoryReader() : null;
        }

        public static IImageReader? GetImageReader(IStorageFile file)
        {
            if (file.FileType == ".pvr" || file.FileType == ".ccz")
            {
                return new PvrReader();
            }
            return IsImageFile(file) ? new ImageFactoryReader() : null;
        }

        public static IPluginReader? GetLayerReader(IStorageFile file)
        {
            if (file.FileType == ".tres")
            {
                return new TresReader();
            }
            if (file.FileType == ".plist")
            {
                return new PlistReader();
            }
            return IsLayerFile(file) ? new JsonFactoryReader() : null;
        }

        public static IPluginReader? GetLayerReader(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension == ".tres")
            {
                return new TresReader();
            }
            if (extension == ".plist")
            {
                return new PlistReader();
            }
            return IsLayerFile(fileName) ? new JsonFactoryReader() : null;
        }

        public static async Task<SKBitmap?> LoadImageAsync(string fileName)
        {
            var reader = GetImageReader(fileName);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(fileName);
        }

        public static async Task<SKBitmap?> LoadImageAsync(IStorageFile file)
        {
            var reader = GetImageReader(file);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(file);
        }

        public static async Task<LayerGroupItem?> LoadLayerAsync(IStorageFile file)
        {
            var reader = GetLayerReader(file);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(file);
        }

        public static async Task<LayerGroupItem?> LoadLayerAsync(string fileName)
        {
            var reader = GetLayerReader(fileName);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(fileName);
        }
    }
}
