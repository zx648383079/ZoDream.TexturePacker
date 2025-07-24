using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.TexturePacker.Plugins
{
    public class FileLoader
    {
        public FileLoader()
        {
            
        }
        public FileLoader(IStorageItem item)
            : this([item])
        {
            
        }

        public FileLoader(IEnumerable<IStorageItem> items)
        {
            _storageItems = items;
        }

        private readonly IEnumerable<IStorageItem> _storageItems = [];
        private readonly Dictionary<string, IImageReader> _imageItems = [];
        private readonly Dictionary<string, IPluginReader> _layerItems = [];
        private readonly Dictionary<string, ISkeletonReader> _skeletonItems = [];

        public IEnumerable<string> ResourceItems => [.. _imageItems.Keys, .. _layerItems.Keys, .. _skeletonItems.Keys];

        public async Task<bool> LoadAsync()
        {
            return await LoadAsync(_storageItems);
        }
        public async Task<bool> LoadAsync(IStorageItem item)
        {
            return await LoadAsync([item]);
        }
        public async Task<bool> LoadAsync(IEnumerable<IStorageItem> items)
        {
            Clear();
            await EachFileAsync(items, item => {
                var image = ReaderFactory.GetImageReader(item);
                if (image != null) 
                {
                    _imageItems.Add(item.Path, image);
                    return;
                }
                var layer = ReaderFactory.GetSpriteReader(item);
                if (layer != null)
                {
                    _layerItems.Add(item.Path, layer);
                }
                var skeleton = ReaderFactory.GetSkeletonReader(item);
                if (skeleton != null)
                {
                    _skeletonItems.Add(item.Path, skeleton);
                }
            });
            return _imageItems.Count > 0 || _layerItems.Count > 0 || _skeletonItems.Count > 0;
        }

        public Task<bool> LoadAsync(IEnumerable<string> items)
        {
            Clear();
            EachFileAsync(items, item => {
                var image = ReaderFactory.GetImageReader(item);
                if (image != null)
                {
                    _imageItems.Add(item, image);
                    return;
                }
                var layer = ReaderFactory.GetSpriteReader(item);
                if (layer != null)
                {
                    _layerItems.Add(item, layer);
                }
                var skeleton = ReaderFactory.GetSkeletonReader(item);
                if (skeleton != null)
                {
                    _skeletonItems.Add(item, skeleton);
                }
            });
            return Task.FromResult(_imageItems.Count > 0 || _layerItems.Count > 0 || _skeletonItems.Count > 0);
        }

        public async IAsyncEnumerable<ImageSection> EnumerateImage()
        {
            foreach (var item in _imageItems)
            {
                var res = await item.Value.ReadAsync(item.Key);
                if (res is null)
                {
                    continue;
                }
                var metaItems = await ReaderFactory.LoadImageMetaAsync(item.Key);
                yield return new ImageSection(item.Key, res, [..metaItems]);
            }
        }

        public async IAsyncEnumerable<SpriteLayerSection> EnumerateLayer()
        {
            foreach (var item in _layerItems)
            {
                var res = await item.Value.ReadAsync(item.Key);
                if (res is null)
                {
                    continue;
                }
                var name = Path.GetFileNameWithoutExtension(item.Key);
                foreach (var it in res)
                {
                    if (it is null)
                    {
                        continue;
                    }
                    if (it.UseCustomName)
                    {
                        it.Name = name;
                    }
                    yield return it;
                }
            }
        }

        public async IAsyncEnumerable<ISkeleton> EnumerateSkeleton()
        {
            foreach (var item in _skeletonItems)
            {
                var res = await item.Value.ReadAsync(item.Key);
                if (res is null)
                {
                    continue;
                }
                foreach (var it in res)
                {
                    if (it is null)
                    {
                        continue;
                    }
                    yield return it;
                }
            }
        }

        public void Clear()
        {
            _imageItems.Clear();
            _layerItems.Clear();
            _skeletonItems.Clear();
        }

        private static async Task EachFileAsync(
            IEnumerable<IStorageItem> items, 
            Action<IStorageFile> cb, CancellationToken token = default)
        {
            foreach (var item in items)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                if (item is IStorageFile file)
                {
                    cb(file);
                    continue;
                }
                if (item is IStorageFolder folder)
                {
                    await EachFileAsync(await folder.GetItemsAsync(), cb, token);
                }
            }
        }

        private static void EachFileAsync(
            IEnumerable<string> items,
            Action<string> cb, CancellationToken token = default)
        {
            var options = new EnumerationOptions()
            {
                RecurseSubdirectories = true,
                MatchType = MatchType.Win32,
                AttributesToSkip = System.IO.FileAttributes.None,
                IgnoreInaccessible = false
            };
            foreach (var item in items)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                if (File.Exists(item))
                {
                    cb(item);
                    continue;
                }
                var res = new FileSystemEnumerable<string>(item, delegate (ref FileSystemEntry entry)
                {
                    return entry.ToSpecifiedFullPath();
                }, options)
                {
                    ShouldIncludePredicate = delegate (ref FileSystemEntry entry)
                    {
                        return !entry.IsDirectory;
                    }
                };
                foreach (var it in res)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    cb(it);
                }
            }
        }
    }
}
