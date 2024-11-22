using System.Windows.Input;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class RenameDialogViewModel : BindableBase, IFormValidator
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
                IsValid = !string.IsNullOrWhiteSpace(value);
            }
        }

        private bool _isValid;

        public bool IsValid {
            get => _isValid;
            set => Set(ref _isValid, value);
        }


        public ICommand TextChangedCommand { get; private set; }

        private void OnTextChanged(bool changed)
        {
            IsValid = changed;
        }
    }
}
