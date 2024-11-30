using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageComputedStyler(IImageStyler styler) : IImageComputedStyler, IImageSize
    {
        public string Name => string.Empty;

        public float Width { get; set; }
        public float Height { get; set; }

        public float ActualWidth => Width;

        public float ActualHeight => Height;

        private int _zIndex = 0;

        private readonly Dictionary<int, IImageComputedStyle> _cacheItems = [];

        private void Add(IImageLayer layer, IImageStyle style)
        {
            Add(layer.Id, style);
        }

        private void Add(int id, IImageStyle style)
        {
            var computed = style is IImageComputedStyle c ? c : new ImageComputedStyle(style);
            Width = Math.Max(Width, computed.ActualOuterWidth);
            Height = Math.Max(Height, computed.ActualOuterHeight);
            computed.ZIndex = _zIndex --;
            if (!_cacheItems.TryAdd(id, computed))
            {
                _cacheItems[id] = computed;
            }
        }

        public void Compute(IImageLayerTree items)
        {
            foreach (var item in items)
            {
                if (item.IsVisible || item.IsChildrenEnabled)
                {
                    Compute(item);
                }
            }
        }

        public IImageStyle Compute(IImageLayer layer)
        {
            if (_cacheItems.TryGetValue(layer.Id, out var style))
            {
                return style;
            }
            ComputeSize(layer);
            return _cacheItems[layer.Id];
        }

        private (float, float) ComputeSize(IImageLayer layer)
        {
            var width = 0f;
            var height = 0f;
            if (layer.IsChildrenEnabled)
            {
                var (w, h) = ComputeSize(layer.Children);
                width = Math.Max(width, w);
                height = Math.Max(height, h);
            }
            var style = styler.Compute(layer);
            if (layer.IsVisible)
            {
                width = Math.Max(width, style.Width);
                height = Math.Max(height, style.Height);
            }
            style.Width = width;
            style.Height = height;
            if (style is IImageComputedStyle s)
            {
                s.Compute();
                width = s.ActualOuterWidth;
                height = s.ActualOuterHeight;
            }
            Add(layer, style);
            return (width, height);
        }

        private (float, float) ComputeSize(IEnumerable<IImageLayer> items)
        {
            var width = 0f;
            var height = 0f;
            foreach (var item in items)
            {
                var (w, h) = ComputeSize(item);
                width = Math.Max(width, w);
                height = Math.Max(height, h);
            }
            return (width, height);
        }

        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKPoint point)
        {
            foreach (var item in _cacheItems.OrderByDescending(i => i.Value.ZIndex))
            {
                if (!item.Value.ToRect().Contains(point))
                {
                    continue;
                }
                var layer = items.Get(item.Key);
                if (layer is null || !layer.IsVisible)
                {
                    continue;
                }
                yield return layer;
            }
        }

        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKRect rect)
        {
            foreach (var item in _cacheItems.OrderByDescending(i => i.Value.ZIndex))
            {
                if (!item.Value.ToRect().IntersectsWith(rect))
                {
                    continue;
                }
                var layer = items.Get(item.Key);
                if (layer is null || !layer.IsVisible)
                {
                    continue;
                }
                yield return layer;
            }
        }

        public void Paint(IImageLayerTree items, IImageCanvas canvas)
        {
            items.Paint(canvas);
        }
        public void Paint(IImageLayerTree items, SKCanvas canvas)
        {
            Paint(items, new ImageCanvas(canvas, this));
        }

        public void Clear()
        {
            _cacheItems.Clear();
            Width = 0;
            Height = 0;
        }

        
    }
}
