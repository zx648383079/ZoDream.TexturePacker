using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.TexturePacker.Dialogs;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
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
