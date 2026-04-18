using System.Windows.Input;
using ZoDream.TexturePacker.ViewModels.Models;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private PluginMenuItem[] _pluginMenuItems = [];

        public PluginMenuItem[] PluginMenuItems {
            get => _pluginMenuItems;
            set => Set(ref _pluginMenuItems, value);
        }

        public ICommand ExitCommand { get; private set; }

        private void TapExit(object? _)
        {
            App.Current.Exit();
        }

    }
}
