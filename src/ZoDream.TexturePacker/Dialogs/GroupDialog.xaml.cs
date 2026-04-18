using Microsoft.UI.Xaml.Controls;
using ZoDream.TexturePacker.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ZoDream.TexturePacker.Dialogs
{
    public sealed partial class GroupDialog : ContentDialog
    {
        public GroupDialog()
        {
            this.InitializeComponent();
        }

        public RenameDialogViewModel ViewModel => (RenameDialogViewModel)DataContext;
    }
}
