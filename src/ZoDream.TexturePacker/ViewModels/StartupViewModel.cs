﻿using System;
using System.Windows.Input;
using Windows.Storage.Pickers;
using ZoDream.Shared.ViewModel;
using ZoDream.TexturePacker.Pages;
using ZoDream.TexturePacker.Plugins;

namespace ZoDream.TexturePacker.ViewModels
{
    internal class StartupViewModel: BindableBase
    {
        public StartupViewModel()
        {
            OpenCommand = new RelayCommand(TapOpen);
            CreateCommand = new RelayCommand(TapCreate);
            version = App.ViewModel.Version;
        }

        private string version;

        public string Version {
            get => version;
            set => Set(ref version, value);
        }

        public ICommand OpenCommand { get; private set; }

        public ICommand CreateCommand { get; private set; }

        private async void TapOpen(object? _)
        {
            var picker = new FileOpenPicker();
            foreach (var ext in ReaderFactory.FileFilterItems)
            {
                picker.FileTypeFilter.Add(ext);
            }
            picker.FileTypeFilter.Add("*");
            App.ViewModel.InitializePicker(picker);
            var items = await picker.PickMultipleFilesAsync();
            if (items.Count == 0)
            {
                return;
            }
            App.ViewModel.Navigate<WorkspacePage>(items);
        }

        private void TapOpenFolder(object? _)
        {
            //var picker = new FolderPicker();
            //App.ViewModel.InitializePicker(picker);
            //picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            //var folder = await picker.PickSingleFolderAsync();
            //if (folder is null)
            //{
            //    return;
            //}
            //var checkFile = await folder.GetFileAsync(AppConstants.DatabaseFileName);
            //if (checkFile is null)
            //{
            //    // 不存在
            //    return;
            //}
            //StorageApplicationPermissions.FutureAccessList.AddOrReplace(AppConstants.WorkspaceToken, folder);
            //await App.GetService<AppViewModel>().InitializeWorkspaceAsync(folder);
            //App.GetService<IRouter>().GoToAsync(Router.HomeRoute);
        }

        private void TapCreate(object? _)
        {
            App.ViewModel.Navigate<WorkspacePage>();
        }
    }
}
