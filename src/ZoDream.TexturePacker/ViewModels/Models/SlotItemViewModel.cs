using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class SlotItemViewModel : BindableBase
    {


        public string Name { get; set; } = string.Empty;

        public int FrameCount { get; set; }

        private int _frameIndex;

        public int FrameIndex {
            get => _frameIndex;
            set => Set(ref _frameIndex, value);
        }

    }
}
