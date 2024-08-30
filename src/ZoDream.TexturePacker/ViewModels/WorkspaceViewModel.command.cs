using SkiaSharp;
using System;
using System.Reflection.Emit;
using System.Windows.Input;
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
            await App.ViewModel.OpenDialogAsync(dialog);
        }

        private async void TapAddGroup(object? _)
        {
            var dialog = new GroupDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }

        private void TapUngroup(object? _)
        {
        }

        private async void TapLayerProperty(object? _)
        {
            var dialog = new LayerPropertyDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }

        



        private async void TapAbout(object? _)
        {
            var dialog = new AboutDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }

        private void TapNew(object? _)
        {
            LayerItems.Clear();
            Editor?.Clear();
            Editor?.Unselect();
            Editor?.Invalidate();
            
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
            Editor!.Unselect();
            SelectedLayer = null;
        }
        private void TapOrder(object? arg)
        {
            if (Enum.TryParse<CssSpritesAlgorithm>(arg as string, out var res))
            {
                var (width, height) = new CssSprites(res).Compute([.. Editor!.LayerItems]);
                Editor!.Resize(width, height);
                Editor.Invalidate();
            }
        }

        private void TapTransparent(object? _)
        {
            Editor!.Backgound = Editor.Backgound is null ? SKColors.White : null;
            Editor.Invalidate();
        }

        private void OnEditorSelected(int id)
        {
            SelectedLayer = GetLayer(id);
        }

        private void OnLayerSelected(LayerViewModel? layer)
        {
            if (layer is null)
            {
                return;
            }
            Editor?.Select(layer.Id);
        }



        private void TapSelectTop(object? _)
        {

        }

        private void TapSelectBottom(object? _)
        {

        }

        private void TapSelectParent(object? _)
        {
            
        }

        private void TapSelectPrevious(object? _)
        {

        }

        private void TapSelectNext(object? _)
        {

        }

        private void TapLayerRotate(object? _)
        {

        }

        private void TapLayerScale(object? _)
        {

        }

        private void TapLayerScaleX(object? _)
        {

        }

        private void TapLayerScaleY(object? _)
        {

        }

        private void TapDeleteLayer(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            if (LayerItems.Remove(layer))
            {
                Editor?.Remove(layer.Id);
            }
            Editor?.Invalidate();
        }

        private void TapLayerVisible(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsVisible = true;
            Editor[layer.Id].Visible = true;
            Editor.Invalidate();
        }

        private void TapLayerHidden(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsVisible = false;
            Editor[layer.Id].Visible = false;
            Editor?.Invalidate();
        }

        private void TapAllVisible(object? _)
        {
            LayerItems.Get(item => {
                item.IsVisible = true;
                Editor[item.Id].Visible = false;
                return false;
            });
            Editor?.Invalidate();
        }

        private void TapOtherHidden(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            LayerItems.Get(item => {
                if (item.Children.Count > 0)
                {
                    return false;
                }
                item.IsVisible = item == layer;
                Editor[item.Id].Visible = item.IsVisible;
                return false;
            });
            Editor?.Invalidate();
        }

        private void TapOtherVisible(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            LayerItems.Get(item => {
                if (item.Children.Count > 0)
                {
                    return false;
                }
                item.IsVisible = item != layer;
                Editor[item.Id].Visible = item.IsVisible;
                return false;
            });
            Editor?.Invalidate();
        }

        private void TapLayerVisibleToggle(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsVisible = !layer.IsVisible;
            Editor[layer.Id].Visible = layer.IsVisible;
            Editor?.Invalidate();
        }

        private void TapLayerLockToggle(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsLocked = !layer.IsLocked;
        }

        private void TapLayerLock(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsLocked = true;
        }

        private void TapLayerUnlock(object? arg)
        {
            var layer = arg is LayerViewModel o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsLocked = false;
        }

        private void TapAllUnlock(object? _)
        {
            LayerItems.Get(item => {
                item.IsLocked = false;
                return false;
            });
        }

        private async void TapLayerRename(object? _)
        {
            var dialog = new RenameDialog();
            await App.ViewModel.OpenDialogAsync(dialog);
        }

        private void TapLayerHorizontalLeft(object? _)
        {

        }

        private void TapLayerHorizontalCenter(object? _)
        {

        }

        private void TapLayerHorizontalRight(object? _)
        {

        }

        private void TapLayerVerticalTop(object? _)
        {

        }

        private void TapLayerVerticalMid(object? _)
        {

        }

        private void TapLayerVerticalBottom(object? _)
        {

        }

        private void TapLayerHorizontalFlip(object? _)
        {

        }

        private void TapLayerVerticalFlip(object? _)
        {

        }

        private void TapLayerMoveTop(object? _)
        {

        }

        private void TapLayerMoveUp(object? _)
        {

        }

        private void TapLayerMoveDown(object? _)
        {

        }
        private void TapLayerMoveBottom(object? _)
        {

        }



    }
}
