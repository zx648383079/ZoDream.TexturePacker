using System.Collections.Generic;
using Windows.Storage;
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
            OrderCommand = new RelayCommand(TapOrder);
            AboutCommand = new RelayCommand(TapAbout);
            ExitCommand = new RelayCommand(TapExit);
            LayerSelectedCommand = new RelayCommand<LayerViewModel>(OnLayerSelected);
            EditorSelectedCommand = new RelayCommand<int>(OnEditorSelected);
            DragImageCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnDragImage);
        }
    }
}
