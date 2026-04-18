using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.DataTransfer;
using ZoDream.TexturePacker.ViewModels;
using ZoDream.TexturePacker.ViewModels.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ZoDream.TexturePacker.Dialogs
{
    public sealed partial class PluginDialog : ContentDialog
    {
        public PluginDialog()
        {
            this.InitializeComponent();
        }

        internal PluginDialogViewModel ViewModel => (PluginDialogViewModel)DataContext;

        public PluginMenuItem[] ItemsSource 
        {
            set {
                PluginPanel.ItemsSource = value;
            }
        }
        public PluginMenuItem? SelectedItem => (PluginMenuItem)PluginPanel.SelectedItem;

        public bool FileNameEnabled {
            set {
                OpenPanel.Visibility = value ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }

        public bool OutputEnabled {
            set {
                OutputPanel.Visibility = value ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }

        public string FileName => ViewModel.FileName;

        private void TextBox_DragOver(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;
        }

        private async void TextBox_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                return;
            }
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 0)
            {
                return;
            }
            (sender as TextBox).Text = items[0].Path;
        }
    }
}
