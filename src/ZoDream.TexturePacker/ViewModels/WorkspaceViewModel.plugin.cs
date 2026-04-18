using System;
using Windows.Storage.Pickers;
using ZoDream.Shared.Interfaces;
using ZoDream.TexturePacker.Plugins;
using ZoDream.TexturePacker.ViewModels.Models;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private void TapPlugin(PluginMenuItem plugin)
        {
            switch (plugin.Group)
            {
                case PluginMenuItem.ImportName:
                    TapImportPlugin(plugin);
                    break;
                default:
                    break;
            }
        }

        private async void TapImportPlugin(PluginMenuItem plugin, string fileName = "")
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var picker = new FileOpenPicker();
                foreach (var ext in ReaderFactory.FileFilterItems)
                {
                    picker.FileTypeFilter.Add(ext);
                }
                picker.FileTypeFilter.Add("*");
                _app.InitializePicker(picker);
                var res = await picker.PickSingleFileAsync();
                if (res is null)
                {
                    return;
                }
                fileName = res.Path;
            }
            IsLoading = true;
            var reader = Activator.CreateInstance(plugin.InstanceType);
            switch (reader)
            {
                case IPluginReader pr:
                    await foreach (var item in FileLoader.EnumerateLayer(pr, fileName))
                    {
                        await ImportSpriteAsync(item);
                    }
                    break;
            }
            Instance!.Resize();
            Instance.Invalidate();
            IsLoading = false;
        }
    }
}
