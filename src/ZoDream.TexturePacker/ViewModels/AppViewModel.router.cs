using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using WinRT.Interop;
using ZoDream.TexturePacker.Dialogs;
using Microsoft.UI.Windowing;
using Microsoft.UI.Dispatching;
using System.Reflection.Metadata;

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
