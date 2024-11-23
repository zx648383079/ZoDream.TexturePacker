using System;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.ImageEditor;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private void TapCut(object? _)
        {
        }
        private void TapCopy(object? _)
        {
        }
        private async void TapPaste(object? _)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.StorageItems))
            {
                OnDragImage(await package.GetStorageItemsAsync());
            } else if (package.Contains(StandardDataFormats.Bitmap)) 
            {
                var img = await package.GetBitmapAsync();
                var source = await img.OpenReadAsync();
                AddImage(new StreamImageData(source.AsStreamForRead()));
            }
            else if (package.Contains(StandardDataFormats.WebLink))
            {
                var text = await package.GetWebLinkAsync();
            }
            //else if (package.Contains(StandardDataFormats.UserActivityJsonArray))
            //{
            //}
            else if (package.Contains(StandardDataFormats.ApplicationLink))
            {
                var text = await package.GetApplicationLinkAsync();
            }
            else if (package.Contains(StandardDataFormats.Uri))
            {
                var text = await package.GetUriAsync();
            }
            else if (package.Contains(StandardDataFormats.Html))
            {
                var text = await package.GetHtmlFormatAsync();
            }
            else if (package.Contains(StandardDataFormats.Rtf))
            {
                var text = await package.GetRtfAsync();
            }
            else if (package.Contains(StandardDataFormats.Text))
            {
                // 链接也是在这里
                var text = await package.GetTextAsync();
                if (text.Contains("<svg"))
                {
                    AddImage(new SvgImageData(text));
                }
            }

        }
    }
}
