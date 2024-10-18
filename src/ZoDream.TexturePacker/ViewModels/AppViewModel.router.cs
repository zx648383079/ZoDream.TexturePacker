using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ZoDream.TexturePacker.ViewModels
{
    internal partial class AppViewModel
    {
        
        private Frame _rootFrame;

        private MenuBar? _menuBar;

        public void Navigate<T>() where T : Page
        {
            _rootFrame.Navigate(typeof(T));
        }

        public void Navigate<T>(object parameter) where T : Page
        {
            _rootFrame.Navigate(typeof(T), parameter);
        }

        internal void BindMenu(WorkspaceViewModel viewModel)
        {
            if (_menuBar is null)
            {
                return;
            }
            _menuBar.Visibility = Visibility.Visible;
            _menuBar.DataContext = viewModel;
        }
    }
}
