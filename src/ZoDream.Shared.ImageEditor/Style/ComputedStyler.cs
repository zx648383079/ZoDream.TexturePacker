using System;
using System.Collections.Generic;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageComputedStyler(IImageStyler styler) : IImageComputedStyler, IImageSize
    {
        public string Name => string.Empty;

        public int Width { get; set; }
        public int Height { get; set; }

        public int ActualWidth => Width;

        public int ActualHeight => Height;

        private readonly Dictionary<int, IImageStyle> _cacheItems = [];

        private void Add(IImageLayer layer, IImageStyle style)
        {
            Add(layer.Id, style);
        }

        private void Add(int id, IImageStyle style)
        {
            var computed = style is IImageComputedStyle c ? c : new ImageComputedStyle(style);
            Width = Math.Max(Width, computed.ActualOuterWidth);
            Height = Math.Max(Height, computed.ActualOuterHeight);
            if (_cacheItems.ContainsKey(id))
            {
                _cacheItems[id] = computed;
            } else
            {
                _cacheItems.Add(id, computed);
            }
        }

        public void Compute(IEnumerable<IImageLayer> items)
        {
            foreach (var item in items)
            {
                if (item.IsVisible)
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

        private (int, int) ComputeSize(IImageLayer layer)
        {
            var width = 0;
            var height = 0;
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

        private (int, int) ComputeSize(IEnumerable<IImageLayer> items)
        {
            var width = 0;
            var height = 0;
            foreach (var item in items)
            {
                var (w, h) = ComputeSize(item);
                width = Math.Max(width, w);
                height = Math.Max(height, h);
            }
            return (width, height);
        }

        public void Clear()
        {
            _cacheItems.Clear();
            Width = 0;
            Height = 0;
        }
    }
}
