using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ImageEditor;
using ZoDream.TexturePacker.Dialogs;
using ZoDream.TexturePacker.Plugins;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {

        public ICommand DragImageCommand { get; private set; }
        public ICommand EditorSelectedCommand { get; private set; }
        public ICommand LayerSelectedCommand { get; private set; }

        public ICommand NewCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand PropertyCommand { get; private set; }
        public ICommand UnselectCommand { get; private set; }
        public ICommand SelectTopCommand { get; private set; }
        public ICommand SelectBottomCommand { get; private set; }
        public ICommand SelectParentCommand { get; private set; }
        public ICommand SelectPreviousCommand { get; private set; }
        public ICommand SelectNextCommand { get; private set; }
        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand TransparentCommand { get; private set; }

        public ICommand SeparateCommand { get; private set; }
        public ICommand OrderCommand { get; private set; }
        public ICommand AddLayerCommand { get; private set; }
        public ICommand AddGroupCommand { get; private set; }
        public ICommand UngroupCommand { get; private set; }


        public ICommand ImportFolderCommand { get; private set; }
        public ICommand DeleteLayerCommand { get; private set; }

        public ICommand LayerPropertyCommand { get; private set; }
        public ICommand LayerRotateCommand { get; private set; }
        public ICommand LayerScaleCommand { get; private set; }
        public ICommand LayerScaleXCommand { get; private set; }
        public ICommand LayerScaleYCommand { get; private set; }
        public ICommand LayerVisibleCommand { get; private set; }
        public ICommand LayerVisibleToggleCommand { get; private set; }
        public ICommand LayerHiddenCommand { get; private set; }
        public ICommand AllVisibleCommand { get; private set; }
        public ICommand OtherHiddenCommand { get; private set; }
        public ICommand OtherVisibleCommand { get; private set; }
        public ICommand LayerLockCommand { get; private set; }
        public ICommand LayerLockToggleCommand { get; private set; }
        public ICommand LayerUnlockCommand { get; private set; }
        public ICommand AllUnlockCommand { get; private set; }
        public ICommand LayerRenameCommand { get; private set; }
        public ICommand LayerHorizontalLeftCommand { get; private set; }
        public ICommand LayerHorizontalCenterCommand { get; private set; }
        public ICommand LayerHorizontalRightCommand { get; private set; }

        public ICommand LayerVerticalTopCommand { get; private set; }
        public ICommand LayerVerticalMidCommand { get; private set; }
        public ICommand LayerVerticalBottomCommand { get; private set; }
        public ICommand LayerHorizontalFlipCommand { get; private set; }
        public ICommand LayerVerticalFlipCommand { get; private set; }
        public ICommand LayerMoveTopCommand { get; private set; }
        public ICommand LayerMoveUpCommand { get; private set; }
        public ICommand LayerMoveDownCommand { get; private set; }
        public ICommand LayerMoveBottomCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }

        private async void TapAddLayer(object? _)
        {
            var dialog = new LayerDialog();
            var model = dialog.ViewModel;
            var res = await App.ViewModel.OpenFormAsync(dialog);
            if (!res)
            {
                return;
            }
            Instance?.AddText(model.Text, model.FamilyName, model.Size, model.Foreground.ToSKColor());
            Instance?.Invalidate();
        }

        private async void TapAddGroup(object? _)
        {
            var dialog = new GroupDialog();
            var res = await App.ViewModel.OpenFormAsync(dialog);
            if (!res)
            {
                return;
            }
            Instance?.AddFolder(dialog.ViewModel.Name);
        }

        

        private async void TapAbout(object? _)
        {
            var dialog = new AboutDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }

        private void TapNew(object? _)
        {
            LayerItems.Clear();
            Instance?.Clear();
            Instance?.Unselect();
            Instance?.Invalidate();
            
        }

        private void TapOpen(object? _)
        {
            TapNew(_);
            TapImportFile(_);
        }

        private void TapUndo(object? _)
        {
            UndoRedo.Undo();
        }
        private void TapRedo(object? _) 
        {
            UndoRedo.ReverseUndo();
        }
        
        private async void TapProperty(object? _)
        {
            var dialog = new PropertyDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }
        private void TapUnselect(object? _)
        {
            Instance!.Unselect();
            SelectedLayer = null;
        }
        private void TapOrder(object? arg)
        {
            if (Enum.TryParse<CssSpritesAlgorithm>(arg as string, out var res))
            {
                var (width, height) = new CssSprites(res).Compute([.. Instance!.LayerItems.Select(i => i.Source)]);
                Instance!.Resize(width, height);
                Instance.Invalidate();
            }
        }
        /// <summary>
        /// 自动分离物体对象
        /// </summary>
        /// <param name="_"></param>
        private async void TapSeparate(IImageLayer? arg)
        {
            if (Instance is null || LayerItems.Count == 0)
            {
                return;
            }
            var layer = arg is not null ? arg : SelectedLayer;
            if (layer is null)
            {
                foreach (var item in LayerItems)
                {
                    if (!item.IsVisible || item.IsLocked || item.Children.Count > 0)
                    {
                        continue;
                    }
                    if (item.Source is not BitmapImageSource)
                    {
                        continue;
                    }
                    layer = item;
                    break;
                }
            }
            if(layer is null || layer.Source is not BitmapImageSource)
            {
                return;
            }
            if (layer.Children.Count == 0)
            {
                SeparateImage(layer);
                return;
            }
            var res = await App.ViewModel.ConfirmAsync("是否重新采样子图形？");
            if (!res)
            {
                SeparateImage(layer);
                return;
            }
            SeparateImageAndMerge(layer);
        }

        private void TapTransparent(object? _)
        {
            Instance!.BackgroundColor = Instance.BackgroundColor is null ? SKColors.White : null;
            Instance.Invalidate();
        }

        private void OnEditorSelected(IImageLayer? layer)
        {
            SelectedLayer = layer;
        }

        private void OnLayerSelected(IImageLayer? layer)
        {
            if (layer is null)
            {
                return;
            }
            Instance?.Select(layer);
        }



        private void TapSelectTop(object? _)
        {
            if (LayerItems.Count == 0)
            {
                return;
            }
            var layer = SelectedLayer;
            if (layer is null || layer.Parent is null)
            {
                SelectedLayer = LayerItems[0];
            } else
            {
                SelectedLayer = layer.Parent.Children[0];
            }
            Instance?.Select(SelectedLayer);
        }

        private void TapSelectBottom(object? _)
        {
            if (LayerItems.Count == 0)
            {
                return;
            }
            var layer = SelectedLayer;
            if (layer is null || layer.Parent is null)
            {
                SelectedLayer = LayerItems.Last();
            }
            else
            {
                SelectedLayer = layer.Parent.Children.Last();
            }
            Instance?.Select(SelectedLayer);
        }

        private void TapSelectParent(object? _)
        {
            if (LayerItems.Count == 0 || SelectedLayer is null)
            {
                return;
            }
            var layer = SelectedLayer.Parent;
            if (layer is null)
            {
                return;
            }
            SelectedLayer = layer;
            Instance?.Select(layer);
        }

        private void TapSelectPrevious(object? _)
        {
            if (LayerItems.Count == 0 || SelectedLayer is null)
            {
                return;
            }
            var layer = SelectedLayer;
            if (layer.Parent is null)
            {
                SelectLayer(LayerItems.IndexOf(layer) - 1, LayerItems);
            } else
            {
                SelectLayer(layer.Parent.Children.IndexOf(layer) - 1, layer.Parent.Children);
            }
        }

        private void TapSelectNext(object? _)
        {
            if (LayerItems.Count == 0 || SelectedLayer is null)
            {
                return;
            }
            var layer = SelectedLayer;
            if (layer.Parent is null)
            {
                SelectLayer(LayerItems.IndexOf(layer) + 1, LayerItems);
            }
            else
            {
                SelectLayer(layer.Parent.Children.IndexOf(layer) + 1, layer.Parent.Children);
            }
        }

        private void SelectLayer(int i, IImageLayerTree items)
        {
            if (i < 0 || i >= items.Count)
            {
                return;
            }
            SelectedLayer = items[i];
            Instance?.Select(SelectedLayer);
        }
    }
}
