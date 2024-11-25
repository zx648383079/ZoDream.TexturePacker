using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.ImageEditor;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.TexturePacker.Plugins
{
    public static class ReaderFactory
    {

        private static readonly string[] ImageFilterItems = [".png", ".jpg", 
            ".jpeg", ".webp", ".svg", 
            ".bmp", ".pvr", ".ccz"];

        private static readonly string[] LayerFilterItems = [".json", ".tres", 
            ".moc3", ".atlas", ".txt", ".plist", ".asset"];
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
                ".pvr" or ".ccz" => new Plugin.Readers.TexturePacker.PvrReader(),
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
                return new Plugin.Readers.Unity.AtlasReader();
            }
            return extension switch
            {
                ".tres" => new Plugin.Godot.TresReader(),
                ".plist" => new Plugin.Readers.TexturePacker.PlistReader(),
                ".atlas" => new Plugin.Spine.AtlasReader(),
                ".moc3" => new Plugin.Live2d.MocReader(),
                ".json" => new JsonFactoryReader(),
                ".asset" => new Plugin.Readers.Unity.AssetReader(),
                _ => null,
            };
        }

        private static ISkeletonReader? GetSkeletonExtensionReader(
            string extension, string fileName)
        {
            if (fileName.EndsWith(".skel.txt") || 
                fileName.EndsWith(".skel.json"))
            {
                return new Plugin.Spine.SkeletonJsonReader();
            }
            return extension switch
            {
                ".skel" => new Plugin.Spine.SkeletonReader(),
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

        public static ISkeletonReader? GetSkeletonReader(IStorageFile file)
        {
            return GetSkeletonExtensionReader(file.FileType, file.Name);
        }

        public static ISkeletonReader? GetSkeletonReader(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return GetSkeletonExtensionReader(extension, Path.GetFileName(fileName));
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
            return await reader.ReadAsync(file.Path);
        }

        public static async Task<IEnumerable<SpriteLayerSection>?> LoadSpriteAsync(IStorageFile file)
        {
            var reader = GetSpriteReader(file);
            if (reader is null)
            {
                return null;
            }
            return await reader.ReadAsync(file.Path);
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
                new Plugin.Readers.Unity.MetaReader(),
                new Plugin.Godot.ImportReader(),
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
