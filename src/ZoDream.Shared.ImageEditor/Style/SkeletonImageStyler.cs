using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Stylers;

namespace ZoDream.Shared.ImageEditor
{
    public class SkeletonImageStyler : IImageStyler, IImageComputedStyler, IImageSize, IDisposable
    {
        public SkeletonImageStyler(SkeletonSection skeleton)
            : this (skeleton.Name, new SkeletonController(skeleton))
        {
            
        }

        public SkeletonImageStyler(string name, ISkeletonController controller)
        {
            _name = string.IsNullOrWhiteSpace(name) ? "SKEL_" + DateTime.Now.Ticks : name;
            _controller = controller;
            Width = controller.Width;
            Height = controller.Height;
        }

        private readonly string _name;
        private readonly ISkeletonController _controller;

        private readonly List<ImageComputedStyle> _cacheItems = [];
        private ImageComputedStyle? _lastStyle;

        public string Name => _name;

        public float Width { get; set; }
        public float Height { get; set; }

        public float ActualWidth => Width;

        public float ActualHeight => Height;

        public IImageStyle Compute(IImageLayer layer)
        {
            if (_lastStyle?.LayerId == layer.Id)
            {
                return _lastStyle;
            }
            foreach (var item in _cacheItems)
            {
                if (item.LayerId == layer.Id)
                {
                    return item;
                }
            }
            return new ImageComputedStyle();
        }

        public void Compute(IImageLayerTree items)
        {
            _controller.Update(0);
            foreach (var item in _controller.Items)
            {
                if (item is SpriteUvLayer uv)
                {
                    var layer = items.Get(i => i.Name == uv.Name);
                    if (layer is null)
                    {
                        continue;
                    }
                    var computed = new ImageComputedVertexStyle(layer.Id)
                    {
                        X = item.X,
                        Y = item.Y,
                        Rotate = item.Rotate,
                        ScaleX = item.ScaleX,
                        ScaleY = item.ScaleY,
                        Width = uv.Width,
                        Height = uv.Height,
                        SourceItems = [..uv.VertexItems],//.Select(i => new SKPoint(i.X * Width, i.Y * Height)).ToArray(),
                        PointItems = [.. uv.PointItems]
                    };
                    _cacheItems.Add(computed);
                    continue;
                }
                if (item is SpritePathLayer path)
                {
                    continue;
                }
                if (item is SpriteLayer sprite)
                {
                    var layer = items.Get(i => i.Name == sprite.Name);
                    if (layer is null)
                    {
                        continue;
                    }
                    _cacheItems.Add(new ImageComputedStyle(layer.Id)
                    {
                        X = item.X,
                        Y = item.Y,
                        Rotate = item.Rotate,
                        ScaleX = item.ScaleX,
                        ScaleY = item.ScaleY,
                        ShearX = item.ShearX,
                        ShearY = item.ShearY,
                        Width = sprite.Width,
                        Height = sprite.Height,
                    });
                    continue;
                }
                
            }
        }

        public void Paint(IImageLayerTree items, IImageCanvas canvas)
        {
            foreach (var item in _cacheItems)
            {
                var layer = items.Get(item.LayerId);
                if (layer is null)
                {
                    continue;
                }
                if (!layer.IsVisible)
                {
                    continue;
                }
                layer.Parent?.Source?.Paint(canvas, item);
            }
        }

        public void Paint(IImageLayerTree items, SKCanvas canvas)
        {
            Paint(items, new ImageCanvas(canvas, this));
        }

        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKPoint point)
        {
            foreach (var item in _cacheItems.OrderByDescending(i => i.ZIndex))
            {
                if (!item.ToRect().Contains(point))
                {
                    continue;
                }
                var layer = items.Get(item.LayerId);
                if (layer is null || !layer.IsVisible)
                {
                    continue;
                }
                _lastStyle = item;
                yield return layer;
            }
        }

        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKRect rect)
        {
            foreach (var item in _cacheItems.OrderByDescending(i => i.ZIndex))
            {
                if (!item.ToRect().IntersectsWith(rect))
                {
                    continue;
                }
                var layer = items.Get(item.LayerId);
                if (layer is null || !layer.IsVisible)
                {
                    continue;
                }
                _lastStyle = item;
                yield return layer;
            }
        }

        public void Clear()
        {
            _cacheItems.Clear();
        }

        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}
