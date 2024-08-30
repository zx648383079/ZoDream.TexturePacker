using System;
using System.Collections.ObjectModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class LayerTree: ObservableCollection<LayerViewModel>
    {

        public new bool Remove(LayerViewModel layer)
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

        public LayerViewModel? Get(int id)
        {
            return Get(item => item.Id == id);
        }

        public LayerViewModel? Get(Func<LayerViewModel, bool> checkFn)
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
    }
}
