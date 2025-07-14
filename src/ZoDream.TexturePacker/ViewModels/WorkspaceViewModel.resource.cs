using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private ObservableCollection<ResourceItemViewModel> _resourceItems = [];

        public ObservableCollection<ResourceItemViewModel> ResourceItems {
            get => _resourceItems;
            set => Set(ref _resourceItems, value);
        }

        private void AddResource(string fileName)
        {
            if (ResourceItems.Where(i => i.FullPath.Equals(fileName)).Any())
            {
                return;
            }
            ResourceItems.Add(new ResourceItemViewModel()
            {
                Name = Path.GetFileName(fileName),
                FullPath = fileName,
            });
        }

        private void AddResource(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                AddResource(item);
            }
        }
    }
}
