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
        private readonly Dictionary<string, ISpriteSection> _layerItems = [];
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
            await EachFileAsync(items, async item => {
                var image = ReaderFactory.GetImageReader(item);
                if (image != null) 
                {
                    _imageItems.Add(item.Path, image);
                    return;
                }
                var layer = ReaderFactory.GetSpriteReader(item);
                if (layer != null)
                {
                    await foreach (var it in EnumerateLayer(layer, item.Path))
                    {
                        _layerItems.Add(item.Path, it);
                    }
                }
                var skeleton = ReaderFactory.GetSkeletonReader(item);
                if (skeleton != null)
                {
                    _skeletonItems.Add(item.Path, skeleton);
                }
            });
            return _imageItems.Count > 0 || _layerItems.Count > 0 || _skeletonItems.Count > 0;
        }

        public async Task<bool> LoadAsync(IEnumerable<string> items)
        {
            Clear();
            await EachFileAsync(items, async item => {
                var image = ReaderFactory.GetImageReader(item);
                if (image != null)
                {
                    _imageItems.Add(item, image);
                    return;
                }
                var layer = ReaderFactory.GetSpriteReader(item);
                if (layer != null)
                {
                    await foreach (var it in EnumerateLayer(layer, item))
                    {
                        _layerItems.Add(item, it);
                    }
                }
                var skeleton = ReaderFactory.GetSkeletonReader(item);
                if (skeleton != null)
                {
                    _skeletonItems.Add(item, skeleton);
                }
            });
            return _imageItems.Count > 0 || _layerItems.Count > 0 || _skeletonItems.Count > 0;
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

        private async IAsyncEnumerable<ISpriteSection> EnumerateLayer(IPluginReader reader, string filePath)
        {
            var res = await reader.ReadAsync(filePath);
            if (res is null)
            {
                yield break;
            }
            var name = Path.GetFileNameWithoutExtension(filePath);
            foreach (var it in res)
            {
                if (it is null)
                {
                    continue;
                }
                if ((it is SpriteLayerSection s && s.UseCustomName) || string.IsNullOrEmpty(it.Name))
                {
                    it.Name = name;
                }
                yield return it;
            }
        }

        public async IAsyncEnumerable<ISpriteSection> EnumerateLayer()
        {
            foreach (var item in _layerItems)
            {
                yield return item.Value;
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
                    if (it is ISkeletonController ctl)
                    {
                        foreach (var layer in _layerItems)
                        {
                            ctl.Connect(layer.Value);
                        }
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
            Func<IStorageFile, Task> cb, CancellationToken token = default)
        {
            foreach (var item in items)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                if (item is IStorageFile file)
                {
                    await cb(file);
                    continue;
                }
                if (item is IStorageFolder folder)
                {
                    await EachFileAsync(await folder.GetItemsAsync(), cb, token);
                }
            }
        }

        private static async Task EachFileAsync(
            IEnumerable<string> items,
            Func<string, Task> cb, CancellationToken token = default)
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
                    await cb(item);
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
                    await cb(it);
                }
            }
        }
    }
}
