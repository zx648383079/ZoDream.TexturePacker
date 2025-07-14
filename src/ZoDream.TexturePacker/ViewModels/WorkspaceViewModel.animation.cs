using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZoDream.Shared.Models;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private ObservableCollection<AnimationItemViewModel> _animationItems = [];

        public ObservableCollection<AnimationItemViewModel> AnimationItems {
            get => _animationItems;
            set => Set(ref _animationItems, value);
        }

        private void AddAnimation(IEnumerable<SkeletonAnimationSection> items)
        {
            foreach (var item in items)
            {
                AnimationItems.Add(new()
                {
                    Name = item.Name,
                });
            }
        }
    }
}
