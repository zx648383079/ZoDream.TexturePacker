using ZoDream.Shared.EditorInterface;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {
        private void TapUngroup(IImageLayer? arg)
        {
            var layer = arg is not null ? arg : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            Instance?.InsertAfter(layer.Children, layer);
            layer.Children.Clear();
            Instance?.Remove(layer);
        }

        private void TapLayerRotate(object? arg)
        {
            var deg = 0;
            if (arg is int i)
            {
                deg = i;
            } else if (arg is string s)
            {
                _ = int.TryParse(s, out deg);
            }
            if (deg == 0 || SelectedLayer is null)
            {
                return;
            }
            SelectedLayer.Source.Rotate(deg);
            Instance?.Invalidate();
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
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            Instance?.Remove(layer);
            if (SelectedLayer == layer)
            {
                SelectedLayer = null;
                Instance?.Unselect();
            }
            Instance?.Invalidate();
        }

        private void TapLayerVisible(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsVisible = true;
            Instance?.Invalidate();
        }

        private void TapLayerHidden(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsVisible = false;
            Instance?.Invalidate();
        }

        private void TapAllVisible(object? _)
        {
            LayerItems.Get(item => {
                item.IsVisible = true;
                return false;
            });
            Instance?.Invalidate();
        }

        private void TapOtherHidden(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
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
                return false;
            });
            Instance?.Invalidate();
        }

        private void TapOtherVisible(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
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
                return false;
            });
            Instance?.Invalidate();
        }

        private void TapLayerVisibleToggle(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsVisible = !layer.IsVisible;
            Instance?.Invalidate();
        }

        private void TapLayerLockToggle(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsLocked = !layer.IsLocked;
        }

        private void TapLayerLock(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.IsLocked = true;
        }

        private void TapLayerUnlock(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
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
