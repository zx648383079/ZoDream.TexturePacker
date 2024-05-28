using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Windows.Storage;
using ZoDream.TexturePacker.ImageEditor;
using ZoDream.TexturePacker.Plugins;
using ZoDream.TexturePacker.Plugins.Readers.Unity;

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

        public ICommand ImportFolderCommand { get; private set; }
        public ICommand DeleteLayerCommand { get; private set; }

        public ICommand LayerPropertyCommand { get; private set; }
        public ICommand LayerRotateCommand { get; private set; }
        public ICommand LayerScaleCommand { get; private set; }
        public ICommand LayerScaleXCommand { get; private set; }
        public ICommand LayerScaleYCommand { get; private set; }
        public ICommand LayerVisibleCommand { get; private set; }
        public ICommand LayerHiddenCommand { get; private set; }
        public ICommand AllVisibleCommand { get; private set; }
        public ICommand OtherHiddenCommand { get; private set; }
        public ICommand OtherVisibleCommand { get; private set; }
        public ICommand LayerLockCommand { get; private set; }
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

        private void TapAddLayer(object? _)
        {

        }

        private void TapLayerProperty(object? _)
        {

        }

        private void TapDeleteLayer(object? _)
        {

        }

        private void TapImportFolder(object? _)
        {

        }

        private void TapAbout(object? _)
        {

        }

        private void TapNew(object? _)
        {

        }

        private void TapOpen(object? _)
        {

        }
        private void TapSave(object? _) 
        {
        }
        private void TapSaveAs(object? _)
        {
        }
        private void TapImport(object? _) 
        {
        }
        private void TapExport(object? _) 
        {
        }
        private void TapUndo(object? _)
        {
        }
        private void TapRedo(object? _) 
        {
        }
        private void TapCut(object? _)
        {
        }
        private void TapCopy(object? _) 
        {
        }
        private void TapPaste(object? _) 
        {
        }
        private void TapProperty(object? _)
        {
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

        private void TapLayerVisible(object? _)
        {

        }

        private void TapLayerHidden(object? _)
        {

        }

        private void TapAllVisible(object? _)
        {

        }

        private void TapOtherHidden(object? _)
        {

        }

        private void TapOtherVisible(object? _)
        {

        }

        private void TapLayerLock(object? _)
        {

        }

        private void TapLayerUnlock(object? _)
        {

        }

        private void TapAllUnlock(object? _)
        {

        }

        private void TapLayerRename(object? _)
        {

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



        private async void OnDragImage(IReadOnlyList<IStorageItem> items)
        {
            var imageLayer = new Dictionary<string, BitmapImageLayer>();
            var packerMap = new Dictionary<string, IStorageFile>();
            foreach (var item in items)
            {
                if (item is not IStorageFile file)
                {
                    continue;
                }
                var name = Path.GetFileNameWithoutExtension(item.Path);
                if (file.ContentType.Contains("image"))
                {
                    var layer = await Editor!.AddImageAsync(file);
                    AddLayer(layer.Id, name, layer.GetPreviewSource());
                    imageLayer.TryAdd(name, layer);
                    continue;
                }
                if (file.ContentType.Contains("json"))
                {
                    packerMap.TryAdd(name, file);
                    continue;
                }
            }
            foreach (var item in packerMap)
            {
                if (!imageLayer.TryGetValue(item.Key, out var layer))
                {
                    continue;
                }
                var data = await new JsonReader().ReadAsync(item.Value);
                if (data is null || data.Items.Count == 0) {
                    continue;
                }
                var parentLayer = GetLayer(layer.Id)!;
                foreach (var kid in data.Items)
                {
                    var kidLayer = layer.Split(kid);
                    Editor!.Add(kidLayer);
                    parentLayer.Children.Add(new LayerViewModel()
                    {
                        Id = kidLayer.Id,
                        Name = kid.Name,
                        PreviewImage = kidLayer.GetPreviewSource()
                    });
                }
                layer.Visible = false;
            }
            // var (width, height) = new CssSprites().Compute([.. Editor.LayerItems]);
            // Editor.Resize(width, height);
            Editor!.Resize();
            Editor.Invalidate();
        }

        
    }
}
