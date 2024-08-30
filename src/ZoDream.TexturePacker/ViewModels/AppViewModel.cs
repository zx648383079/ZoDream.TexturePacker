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
using ZoDream.TexturePacker.Pages;

namespace ZoDream.TexturePacker.ViewModels
{
    internal partial class AppViewModel
    {
        private Window _baseWindow;
        private IntPtr _baseWindowHandle;
        private AppWindow _appWindow;

        /// <summary>
        /// UI线程.
        /// </summary>
        public DispatcherQueue DispatcherQueue => _baseWindow!.DispatcherQueue;

        /// <summary>
        /// 获取当前版本号.
        /// </summary>
        /// <returns>版本号.</returns>
        public string Version {
            get {
                var version = Package.Current.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }


        public void Binding(Window window, Frame frame, MenuBar? menuBar)
        {
            _baseWindow = window;
            _baseWindowHandle = WindowNative.GetWindowHandle(_baseWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(_baseWindowHandle);
            _appWindow = AppWindow.GetFromWindowId(windowId);
            _rootFrame = frame;
            _menuBar = menuBar;
            Navigate<StartupPage>();
        }

        public double GetDpiScaleFactorFromWindow()
        {
            return BaseXamlRoot.RasterizationScale;
        }
    }
}
