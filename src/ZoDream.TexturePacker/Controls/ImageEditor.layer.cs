using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.ImageEditor;
using ZoDream.TexturePacker.Plugins;

namespace ZoDream.TexturePacker.Controls
{
    public partial class ImageEditor : IImageEditor
    {
        private int _idGenerator = 0;
        /// <summary>
        /// 预先的尺寸
        /// </summary>
        private int _widthI = 0;
        private int _heightI = 0;
        private TransparentImageSource? _transparentBackground;

        private ICommandImageSource? _commandLayer;
        private IImageCommander _commander;

        public IImageCommander Commander 
        {
            set {
                _commander = value;
                _commander.Instance = this;
            }
        }

        public IImageComputedStyler ComputedStyler { get; private set; }

        public IImageLayerTree LayerItems => _commander.Source;

        public SKColor? BackgroundColor { get; set; }


        public int ActualHeightI => _heightI;
        public int ActualWidthI => _widthI;

        public IImageLayer? this[int id] => Get(id);


        public IImageLayer? Get(int id)
        {
            return LayerItems.Get(id);
        }

        public IImageLayer? AddImage(SKBitmap? image)
        {
            if (image is null)
            {
                return null;
            }
            return Add(new BitmapImageSource(image, this));
        }

        public IImageLayer? AddImage(IImageData? image)
        {
            var res = image?.TryParse(this);
            if (res is null)
            {
                return null;
            }
            return Add(res);
        }

        public async Task<IImageLayer?> AddImageAsync(string fileName)
        {
            var layer = AddImage(await ReaderFactory.LoadImageAsync(fileName));
            if (layer is not null)
            {
                layer.Name = Path.GetFileNameWithoutExtension(fileName);
            }
            return layer;
        }

        public async Task<IImageLayer?> AddImageAsync(IStorageFile file)
        {
            var layer = AddImage(await ReaderFactory.LoadImageAsync(file));
            if (layer is not null)
            {
                layer.Name = file.Name;
            }
            return layer;
        }

        public void Clear()
        {
            foreach (var item in LayerItems)
            {
                item.Dispose();
            }
            LayerItems.Clear();
        }

        public IImageLayer AddText(string text)
        {
            return Add(new TextImageSource(text, this));
        }

        public IImageLayer AddText(
            string text, 
            string fontFamily, 
            int fontSize, SKColor color)
        {
            return Add(new TextImageSource(text, this)
            {
                FontFamily = SKFontManager.Default.MatchFamily(fontFamily),
                FontSize = fontSize,
                Color = color
            }, text.Trim());
        }

        public IImageLayer AddFolder(string name)
        {
            return Add(new FolderImageSource(this), name);
        }

        public void Add(IEnumerable<IImageLayer?> items)
        {
            foreach (var item in items.Reverse())
            {
                if (item is null)
                {
                    continue;
                }
                Add(item);
            }
        }

        public void Add(IEnumerable<IImageLayer?> items, IImageLayer parent)
        {
            foreach (var item in items.Reverse())
            {
                if (item is null)
                {
                    continue;
                }
                Add(item, parent);
            }
        }

        public IImageLayer Add(IImageSource source)
        {
            var layer = _commander.Create(source);
            Add(layer);
            return layer;
        }

        public IImageLayer Add(IImageSource source, string name)
        {
            var layer = _commander.Create(source);
            layer.Name = name.Length > 10 ? name[0..8] + "..." : name;
            Add(layer);
            return layer;
        }

        public void Add(IImageLayer layer)
        {
            if (LayerItems.Contains(layer))
            {
                return;
            }
            layer.Parent?.Children.Remove(layer);
            Initialize(layer);
            layer.Depth = 0;
            layer.Parent = null;
            LayerItems.AddFirst(layer);
        }

        private void Initialize(IImageLayer layer)
        {
            if (layer.Id < 1) 
            {
                GenerateLayerId(layer);
            }
            if (string.IsNullOrWhiteSpace(layer.Name))
            {
                layer.Name = $"undefined_{layer.Id}";
            }
        }

        public void Add(IImageLayer layer, IImageLayer? parent)
        {
            if (parent is null)
            {
                Add(layer);
                return;
            }
            Initialize(layer);
            layer.Depth = parent.Depth + 1;
            if (parent != layer.Parent)
            {
                if (layer.Parent is null)
                {
                    LayerItems.Remove(layer);
                } else
                {
                    layer.Parent.Children.Remove(layer);
                }
                layer.Parent = parent;
            }
            parent.Children.AddFirst(layer);
        }

        public void InsertAfter(IEnumerable<IImageLayer> items, IImageLayer layer)
        {
            var parent = layer.Parent;
            var tree = parent is null ? LayerItems : parent.Children;
            var i = LayerItems.IndexOf(layer) + 1;
            foreach (var item in items.Reverse())
            {
                item.Parent = parent;
                item.Depth = layer.Depth;
                tree.Insert(i, item);
            }
        }

        public void GenerateLayerId(IImageLayer layer)
        {
            if (layer.Id > 0)
            {
                return;
            }
            layer.Id = ++_idGenerator;
        }

        public void Remove(int id)
        {
            var layer = Get(id);
            if (layer is null)
            {
                return;
            }
            Remove(layer);
        }

        public void Remove(IImageLayer layer)
        {
            if (layer.Parent is null)
            {
                LayerItems.Remove(layer);
            } else
            {
                layer.Parent.Children.Remove(layer);
            }
            layer.Dispose();
        }

        /// <summary>
        /// 自动变更尺寸，显示全部内容
        /// </summary>
        public void Resize()
        {
            var outerWidth = 0f;
            var outerHeight = 0f;
            foreach (var item in LayerItems)
            {
                outerWidth = Math.Max(outerWidth, item.Source.X + item.Source.Width);
                outerHeight = Math.Max(outerHeight, item.Source.Y + item.Source.Height);
            }
            if (outerHeight == 0 || outerWidth == 0)
            {
                return;
            }
            Resize(outerWidth, outerHeight);
        }
        public void Resize(float width, float height)
        {
            _widthI = (int)width; 
            _heightI = (int)height;
            ResizeWithControl(_widthI, _heightI);
            _transparentBackground?.Invalidate();
            //_surface?.Dispose();
            //_surface = null;
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Tap(float x, float y)
        {
            var layer = ComputedStyler.Where(LayerItems, new SKPoint(x, y)).FirstOrDefault();
            if (layer == null)
            {
                return;
            }
            Select(layer);
            SelectedCommand?.Execute(layer);
        }
        public void Select(int id)
        {
            var layer = Get(id);
            if (layer is not null)
            {
                Select(layer);
            }
        }
        public void Select(IImageLayer? layer)
        {
            if (layer is null)
            {
                Unselect();
                return;
            }
            _commandLayer ??= new SelectionImageSource(this);
            _commandLayer.With(layer);
            Invalidate();
        }

        public void Unselect()
        {
            if (_commandLayer is null)
            {
                return;
            }
            _commandLayer?.Dispose();
            _commandLayer = null;
            Invalidate();
        }

        public void Paint(SKCanvas canvas, SKImageInfo info)
        {
            ComputedStyler = Compute();
            if (ComputedStyler.ActualWidth != ActualWidthI || ComputedStyler.ActualHeight != ActualHeightI)
            {
                Resize(ComputedStyler.ActualWidth, ComputedStyler.ActualHeight);
            }
            canvas.Clear(BackgroundColor ?? SKColors.Transparent);
            var c = new ImageCanvas(canvas, ComputedStyler);
            if (BackgroundColor is null)
            {
                _transparentBackground ??= new TransparentImageSource(this);
                _transparentBackground.Paint(c);
            }
            ComputedStyler.Paint(LayerItems, c);
            _commandLayer?.Paint(c);
        }

        public void Invalidate()
        {
            CanvasTarget.Invalidate();
        }
        /// <summary>
        /// 先计算
        /// </summary>
        /// <returns></returns>
        private IImageComputedStyler Compute()
        {
            var source = _commander.Styler;
            var styler = source is IImageComputedStyler s ? 
               s : new ImageComputedStyler(source);
            styler.Clear();
            styler.Compute(LayerItems);
            if ((styler.ActualWidth == 0
                || styler.ActualHeight == 0) 
                && styler is IImageSize c)
            {
                c.Width = ActualWidthI;
                c.Height = ActualHeightI;
            } 
            return styler;
        }

        public void SaveAs(string fileName)
        {
            var styler = Compute();
            using var bitmap = new SKBitmap((int)styler.ActualWidth, (int)styler.ActualHeight);
            using var canvas = new SKCanvas(bitmap);
            styler.Paint(LayerItems, canvas);
            bitmap.SaveAs(fileName);
        }

        public void SaveAs(IImageLayer layer, string fileName)
        {
            if (layer is null)
            {
                return;
            }
            var styler = new ImageComputedStyler(_commander.RealStyler);
            styler.Compute(layer);
            var bitmap = new SKBitmap((int)styler.ActualWidth, (int)styler.ActualHeight);
            using (var surface = new SKCanvas(bitmap))
            {
                var c = new ImageCanvas(surface, styler);
                layer.Paint(c);
            }
            bitmap.SaveAs(fileName);
        }


        public void Dispose()
        {
            foreach (var item in LayerItems)
            {
                item.Dispose();
            }
            _transparentBackground?.Dispose();
        }
    }
}
