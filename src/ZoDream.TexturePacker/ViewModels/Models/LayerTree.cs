using System;
using System.Collections.ObjectModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class LayerTree: ObservableCollection<LayerViewModel>
    {
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
