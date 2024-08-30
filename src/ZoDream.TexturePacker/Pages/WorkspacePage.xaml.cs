using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using Windows.Storage;
using ZoDream.TexturePacker.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ZoDream.TexturePacker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WorkspacePage : Page
    {
        public WorkspacePage()
        {
            InitializeComponent();
        }

        public WorkspaceViewModel ViewModel => (WorkspaceViewModel)DataContext;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.ViewModel.BindMenu(ViewModel);
            ViewModel.Editor = Editor;
            if (e.Parameter is IEnumerable<IStorageItem> items)
            {
                ViewModel.DragFileAsync(items);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.Editor?.Dispose();
        }

    }
}
