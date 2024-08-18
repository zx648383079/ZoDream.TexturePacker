using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class ExportViewModel: BindableBase
    {
        private string[] sourceItems = ["合并文件", "拆分文件", "选中层"];

        public string[] SourceItems {
            get => sourceItems;
            set => Set(ref sourceItems, value);
        }

        private string[] typeItems = ["PNG", "Unity", "Godot", "Egret", "TexturePacker"];

        public string[] TypeItems {
            get => typeItems;
            set => Set(ref typeItems, value);
        }

        private int sourceIndex;

        public int SourceIndex {
            get => sourceIndex;
            set => Set(ref sourceIndex, value);
        }

        private int typeIndex;

        public int TypeIndex {
            get => typeIndex;
            set => Set(ref typeIndex, value);
        }

        private bool resetRotate;

        public bool ResetRotate {
            get => resetRotate;
            set => Set(ref resetRotate, value);
        }

        private bool layerFolder;

        public bool LayerFolder {
            get => layerFolder;
            set => Set(ref layerFolder, value);
        }


        public async Task<IStorageItem> OpenPickerAsync()
        {
            if (SourceIndex < 1)
            {
                var picker = new FileSavePicker();
                picker.FileTypeChoices.Add("Image", [".png", ".jpg", 
                    ".jpeg", ".webp", ".bmp"]);
                picker.FileTypeChoices.Add("KTX", [".ktx"]);
                picker.FileTypeChoices.Add("Windows ICO", [".ico"]);
                App.ViewModel.InitializePicker(picker);
                return await picker.PickSaveFileAsync();
            }
            var folder = new FolderPicker();
            App.ViewModel.InitializePicker(folder);
            return await folder.PickSingleFolderAsync();
        }
    }
}
