using System.Windows.Input;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class RenameDialogViewModel: BindableBase
    {
        public RenameDialogViewModel()
        {
            TextChangedCommand = new RelayCommand<bool>(OnTextChanged);
        }

        private string _name = string.Empty;

        public string Name {
            get => _name;
            set {
                Set(ref _name, value);
                ConfirmEnabled = !string.IsNullOrWhiteSpace(value);
            }
        }

        private bool _confirmEnabled;

        public bool ConfirmEnabled {
            get => _confirmEnabled;
            set => Set(ref _confirmEnabled, value);
        }


        public ICommand TextChangedCommand { get; private set; }

        private void OnTextChanged(bool changed)
        {
            ConfirmEnabled = changed;
        }
    }
}
