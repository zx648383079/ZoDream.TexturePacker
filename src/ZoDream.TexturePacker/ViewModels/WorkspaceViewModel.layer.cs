using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.Extensions;

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
            SelectedLayer.Source.Rotate += deg;
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

        

        private void TapLayerHorizontalLeft(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.Source.X = 0;
        }

        private void TapLayerHorizontalCenter(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var (width, _) = GetParentSize(layer);
            layer.Source.X = (width - layer.Source.Width) / 2;
        }

        private (float, float) GetParentSize(IImageLayer layer)
        {
            var x = 0f;
            var y = 0f;
            var parent = layer.Parent;
            while(parent is not null)
            {
                if (parent.Source.Width > 0 && parent.Source.Height > 0)
                {
                    return (parent.Source.Width - x, parent.Source.Height - y);
                }
                x += parent.Source.X;
                y += parent.Source.Y;
                parent = parent.Parent;
            }
            return (Instance.ActualWidthI - x, Instance.ActualHeightI - y);
        }

        private void TapLayerHorizontalRight(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var (width, _) = GetParentSize(layer);
            layer.Source.X = width - layer.Source.Width;
        }

        private void TapLayerVerticalTop(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.Source.Y = 0;
        }

        private void TapLayerVerticalMid(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var (_, height) = GetParentSize(layer);
            layer.Source.Y = (height - layer.Source.Height) / 2;
        }

        private void TapLayerVerticalBottom(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var (_, height) = GetParentSize(layer);
            layer.Source.Y = height - layer.Source.Height;
        }

        private void TapLayerHorizontalFlip(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.Source.ScaleX *= -1;
        }

        private void TapLayerVerticalFlip(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            layer.Source.ScaleY *= -1;
        }

        private void TapLayerMoveTop(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Parent is null ? LayerItems : layer.Parent.Children;
            items.MoveToFirst(items.IndexOf(layer));
        }

        private void TapLayerMoveUp(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Parent is null ? LayerItems : layer.Parent.Children;
            items.MoveUp(items.IndexOf(layer));
        }

        private void TapLayerMoveDown(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Parent is null ? LayerItems : layer.Parent.Children;
            items.MoveDown(items.IndexOf(layer));
        }
        private void TapLayerMoveBottom(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            var items = layer.Parent is null ? LayerItems : layer.Parent.Children;
            items.MoveToLast(items.IndexOf(layer));
        }

        private void TapLayerMoveParent(object? arg)
        {
            var layer = arg is IImageLayer o ? o : SelectedLayer;
            if (layer is null)
            {
                return;
            }
            if (layer.Parent is null)
            {
                return;
            }
            layer.Source.X += layer.Parent.Source.X;
            layer.Source.Y += layer.Parent.Source.Y;
            layer.Source.Rotate += layer.Parent.Source.Rotate;
            layer.Source.ScaleX *= layer.Parent.Source.ScaleX;
            layer.Source.ScaleY *= layer.Parent.Source.ScaleY;
            layer.Parent.Children.Remove(layer);
            Instance?.InsertAfter([layer], layer.Parent);
        }
    }
}
