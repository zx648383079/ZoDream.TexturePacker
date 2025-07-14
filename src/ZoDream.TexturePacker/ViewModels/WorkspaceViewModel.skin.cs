using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZoDream.Shared.Models;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {


        private ObservableCollection<SkinItemViewModel> _skinItems = [];

        public ObservableCollection<SkinItemViewModel> SkinItems {
            get => _skinItems;
            set => Set(ref _skinItems, value);
        }


        private void AddSkin(IEnumerable<SkeletonSkin> items)
        {
            foreach (var item in items)
            {
                SkinItems.Add(new SkinItemViewModel()
                {
                    Name = item.Name,
                });
            }
        }
    }
}
