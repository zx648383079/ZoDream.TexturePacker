using Microsoft.UI.Xaml.Media.Imaging;
using System;
using ZoDream.Shared.ViewModel;
using ZoDream.Shared.ImageEditor;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.TexturePacker.ViewModels
{
    public class LayerViewModel : BindableBase, IImageLayer
    {

        public LayerViewModel(WorkspaceViewModel workspace, IImageSource source)
        {
            Workspace = workspace;
            Source = source;
            if (source is FolderImageSource f)
            {
                f.Host = this;
            }
        }

        public WorkspaceViewModel Workspace { get; private set; }

        public IImageLayer? Parent { get; set; }

        public int Id { get; set; }

        private string _name = string.Empty;

        public string Name {
            get => _name;
            set => Set(ref _name, value);
        }

        private BitmapSource? _previewImage;

        public BitmapSource? PreviewImage {
            get => _previewImage;
            set => Set(ref _previewImage, value);
        }

        private bool _isVisible = true;

        public bool IsVisible {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        private bool _isLocked;

        public bool IsLocked {
            get => _isLocked;
            set => Set(ref _isLocked, value);
        }

        private IImageLayerTree _children = new LayerTree();

        public IImageLayerTree Children {
            get => _children;
            set => Set(ref _children, value);
        }

        public int Depth { get; set; }

        public IImageSource Source { get; private set; }

        public bool IsChildrenEnabled => IsVisible || Source is not FolderImageSource;


        public IImageLayer? Get(Func<IImageLayer, bool> checkFn)
        {
            if (checkFn(this))
            {
                return this;
            }
            return Children.Get(checkFn);
        }

        public void Resample()
        {
            PreviewImage = Workspace.CreateThumbnail(Source);
        }

        public void Paint(IImageCanvas canvas)
        {
            var isFolder = Source is FolderImageSource;
            if (!IsVisible && (isFolder || Children.Count == 0))
            {
                return;
            }
            var style = canvas.Compute(this);
            canvas.Mutate(style, c => {
                if (IsVisible)
                {
                    Source?.Paint(c, style);
                }
                if (!isFolder)
                {
                    Children.Paint(c.Transform(Source?.X ?? 0, Source?.Y ?? 0));
                }
            });
        }

        public void Dispose()
        {
            foreach (var item in Children)
            {
                item.Dispose();
            }
            Source?.Dispose();
        }

      
    }
}
