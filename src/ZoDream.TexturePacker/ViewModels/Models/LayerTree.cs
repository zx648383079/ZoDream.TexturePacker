using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.TexturePacker.ViewModels
{
    public class LayerTree : ObservableCollection<IImageLayer>, IImageLayerTree, IList<IImageLayer>
    {
        public void AddFirst(IImageLayer layer)
        {
            Insert(0, layer);
        }
        public void AddRange(IEnumerable<IImageLayer> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
        public bool RemoveIfKid(IImageLayer layer)
        {
            var i = IndexOf(layer);
            if (i >= 0)
            {
                RemoveAt(i);
                return true;
            }
            foreach (var item in this)
            {
                if (item.Children.Remove(layer))
                {
                    return true;
                }
            }
            return false;
        }

        public IImageLayer? Get(int id)
        {
            return Get(item => item.Id == id);
        }

        public IImageLayer? Get(Func<IImageLayer, bool> checkFn)
        {
            foreach (var item in this)
            {
                var layer = item.Get(checkFn);
                if (layer is not null)
                {
                    return layer;
                }
            }
            return null;
        }

        public IImageLayer? Get(float x, float y)
        {
            foreach (var item in Items)
            {
                if (!item.IsChildrenEnabled)
                {
                    continue;
                }
                var layer = item.Children.Get(x, y);
                if (layer is not null)
                {
                    return layer;
                }
                if (item.IsVisible && item.Source.Contains(x, y))
                {
                    return item;
                }
            }
            return null;
        }

        public void Paint(IImageCanvas canvas)
        {
            foreach (var item in Items)
            {
                item.Paint(canvas);
            }
        }
    }
}
