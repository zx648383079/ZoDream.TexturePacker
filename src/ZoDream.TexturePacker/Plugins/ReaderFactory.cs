using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Models;
using ZoDream.TexturePacker.Plugins.Readers;
using ZoDream.TexturePacker.Plugins.Readers.Cocos;
using ZoDream.TexturePacker.Plugins.Readers.Godot;
using ZoDream.TexturePacker.Plugins.Readers.Live2d;
using ZoDream.TexturePacker.Plugins.Readers.TexturePacker;

namespace ZoDream.TexturePacker.Plugins
{
    public static class ReaderFactory
    {

        private static string[] ImageFilterItems = [".png", ".jpg", 
            ".jpeg", ".webp", ".svg", 
            ".bmp", ".pvr", ".ccz"];

        private static string[] LayerFilterItems = [".json", ".tres", 
            ".moc3", ".atlas", ".txt", ".plist", ".asset" ];
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

        private static IImageReader? GetImageExtensionReader(string extension)
        {
            IImageReader? reader = extension switch
            {
                ".pvr" or ".ccz" => new PvrReader(),
                ".svg" => new SvgReader(),
                _ => null,
            };
            if (reader is not null)
            {
                return reader;
            }
            if (ImageFilterItems.Contains(extension))
            {
                return new ImageFactoryReader();
            }
            return null;
        }

        public static IImageReader? GetImageReader(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return GetImageExtensionReader(extension);
        }

        public static IImageReader? GetImageReader(IStorageFile file)
        {
            var reader = GetImageExtensionReader(file.FileType);
            if (reader is not null)
            {
                return reader;
            }
            return IsImageFile(file) ? new ImageFactoryReader() : null;
        }

        private static IPluginReader? GetSpriteExtensionReader(
            string extension, string fileName)
        {
            if (fileName.EndsWith(".atlas.txt"))
            {
                return new Readers.Unity.AtlasReader();
            }
            return extension switch
            {
                ".tres" => new TresReader(),
                ".plist" => new PlistReader(),
                ".atlas" => new AtlasReader(),
                ".moc3" => new MocReader(),
                ".json" => new JsonFactoryReader(),
                ".asset" => new Readers.Unity.AssetReader(),
                _ => null,
            };
        }

        public static IPluginReader? GetSpriteReader(IStorageFile file)
        {
            return GetSpriteExtensionReader(file.FileType, file.Name);
        }

        public static IPluginReader? GetSpriteReader(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return GetSpriteExtensionReader(extension, Path.GetFileName(fileName));
        }

        public static async Task<IImageData?> LoadImageAsync(string fileName)
        {
            var reader = GetImageReader(fileName);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(fileName);
        }

        public static async Task<IImageData?> LoadImageAsync(IStorageFile file)
        {
            var reader = GetImageReader(file);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(file);
        }

        public static async Task<IEnumerable<SpriteLayerSection>?> LoadSpriteAsync(IStorageFile file)
        {
            var reader = GetSpriteReader(file);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(file);
        }

        public static async Task<IEnumerable<SpriteLayerSection>?> LoadSpriteAsync(string fileName)
        {
            var reader = GetSpriteReader(fileName);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(fileName);
        }

        public static async Task<IEnumerable<string>> LoadImageMetaAsync(string fileName)
        {
            IFileMetaReader[] metaReaderItems = [
                new Readers.Unity.MetaReader(),
                new Readers.Godot.ImportReader(),
            ];
            foreach (var reader in metaReaderItems)
            {
                var res = await reader.ReadAsync(fileName);
                if (res is not null)
                {
                    return res;
                }
            }
            return [];
        }
    }
}
