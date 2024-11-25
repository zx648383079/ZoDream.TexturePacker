using ZoDream.Shared.UndoRedo;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.TexturePacker.ViewModels
{
    public partial class WorkspaceViewModel
    {

        private readonly AppViewModel _app = App.ViewModel;

        public CommandManager UndoRedo { get; private set; } = new();
        public IImageEditor? Instance { get; set; }


        private bool _undoEnabled;

        public bool UndoEnabled {
            get => _undoEnabled;
            set => Set(ref _undoEnabled, value);
        }

        private bool _redoEnabled;

        public bool RedoEnabled {
            get => _redoEnabled;
            set => Set(ref _redoEnabled, value);
        }

        public bool IsSelectedLayer => SelectedLayer != null;

        

        private IImageLayer? _selectedLayer;

        public IImageLayer? SelectedLayer {
            get => _selectedLayer;
            set {
                Set(ref _selectedLayer, value);
                OnPropertyChanged(nameof(IsSelectedLayer));
            }
        }

        

        
    }
}
