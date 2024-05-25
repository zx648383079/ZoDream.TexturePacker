using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ZoDream.TexturePacker.Controls
{
    public class LayerItemTemplateSelector: DataTemplateSelector
    {
        public DataTemplate? LayerTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            // var layerItem = (LayerViewModel)item;
            if (LayerTemplate is null)
            {
                return base.SelectTemplateCore(item);
            }
            return LayerTemplate;
        }
    }
}
