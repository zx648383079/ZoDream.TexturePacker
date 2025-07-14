using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class ResourceItemViewModel : BindableBase
    {

        public string Name { get; set; } = string.Empty;

        public string FullPath { get; set; } = string.Empty;
    }
}
