using System.Collections.Generic;
using Windows.Storage;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel: BindableBase
    {
        public WorkspaceViewModel()
        {
            NewCommand = new RelayCommand(TapNew);
            OpenCommand = new RelayCommand(TapOpen);
            SaveAsCommand = new RelayCommand(TapSaveAs);
            SaveCommand = new RelayCommand(TapSave);
            ImportCommand = new RelayCommand(TapImport);
            ExportCommand = new RelayCommand(TapExport);
            UndoCommand = new RelayCommand(TapUndo);
            RedoCommand = new RelayCommand(TapRedo);
            CutCommand = new RelayCommand(TapCut);
            PasteCommand = new RelayCommand(TapPaste);
            CopyCommand = new RelayCommand(TapCopy);
            UnselectCommand = new RelayCommand(TapUnselect);
            PropertyCommand = new RelayCommand(TapProperty);
            TransparentCommand = new RelayCommand(TapTransparent);
            SeparateCommand = new RelayCommand<IImageLayer>(TapSeparate);
            OrderCommand = new RelayCommand(TapOrder);
            AboutCommand = new RelayCommand(TapAbout);
            AddLayerCommand = new RelayCommand(TapAddLayer);
            AddGroupCommand = new RelayCommand(TapAddGroup);
            UngroupCommand = new RelayCommand<IImageLayer>(TapUngroup);
            DeleteLayerCommand = new RelayCommand(TapDeleteLayer);
            ImportFolderCommand = new RelayCommand(TapImportFolder);
            LayerPropertyCommand = new RelayCommand(TapLayerProperty);
            SelectTopCommand = new RelayCommand(TapSelectTop);
            SelectBottomCommand = new RelayCommand(TapSelectBottom);
            SelectParentCommand = new RelayCommand(TapSelectParent);
            SelectPreviousCommand = new RelayCommand(TapSelectPrevious);
            SelectNextCommand = new RelayCommand(TapSelectNext);
            LayerRotateCommand = new RelayCommand(TapLayerRotate);
            LayerScaleCommand = new RelayCommand(TapLayerScale);
            LayerScaleXCommand = new RelayCommand(TapLayerScaleX);
            LayerScaleYCommand = new RelayCommand(TapLayerScaleY);
            LayerVisibleCommand = new RelayCommand(TapLayerVisible);
            LayerVisibleToggleCommand = new RelayCommand(TapLayerVisibleToggle);
            LayerLockToggleCommand = new RelayCommand(TapLayerLockToggle);
            LayerHiddenCommand = new RelayCommand(TapLayerHidden);
            AllVisibleCommand = new RelayCommand(TapAllVisible);
            OtherHiddenCommand = new RelayCommand(TapOtherHidden);
            OtherVisibleCommand = new RelayCommand(TapOtherVisible);
            LayerLockCommand = new RelayCommand(TapLayerLock);
            LayerUnlockCommand = new RelayCommand(TapLayerUnlock);
            AllUnlockCommand = new RelayCommand(TapAllUnlock);
            LayerRenameCommand = new RelayCommand(TapLayerRename);
            LayerHorizontalLeftCommand = new RelayCommand(TapLayerHorizontalLeft);
            LayerHorizontalCenterCommand = new RelayCommand(TapLayerHorizontalCenter);
            LayerHorizontalRightCommand = new RelayCommand(TapLayerHorizontalRight);
            LayerVerticalTopCommand = new RelayCommand(TapLayerVerticalTop);
            LayerVerticalMidCommand = new RelayCommand(TapLayerVerticalMid);
            LayerVerticalBottomCommand = new RelayCommand(TapLayerVerticalBottom);
            LayerHorizontalFlipCommand = new RelayCommand(TapLayerHorizontalFlip);
            LayerVerticalFlipCommand = new RelayCommand(TapLayerVerticalFlip);
            LayerMoveTopCommand = new RelayCommand(TapLayerMoveTop);
            LayerMoveUpCommand = new RelayCommand(TapLayerMoveUp);
            LayerMoveDownCommand = new RelayCommand(TapLayerMoveDown);
            LayerMoveBottomCommand = new RelayCommand(TapLayerMoveBottom);
            ExitCommand = new RelayCommand(TapExit);
            LayerSelectedCommand = new RelayCommand<IImageLayer>(OnLayerSelected);
            EditorSelectedCommand = new RelayCommand<IImageLayer>(OnEditorSelected);
            DragImageCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnDragImage);
            UndoRedo.UndoStateChanged += UndoRedo_UndoStateChanged;
            UndoRedo.ReverseUndoStateChanged += UndoRedo_ReverseUndoStateChanged;

        }

        private void UndoRedo_ReverseUndoStateChanged(bool value)
        {
            RedoEnabled = value;
        }

        private void UndoRedo_UndoStateChanged(bool value)
        {
            UndoEnabled = value;
        }
    }
}
