using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class Editor : IDisposable
    {
        public Editor(int width, int height)
        {
            Resize(width, height);
        }
        public int Width { get; set; }
        public int Height { get; set; }

        private int _idGenerator = 0;
        private Action? InvalidateFn;

        private SKSurface? _surface;

        private TransparentImageLayer? _transparentBackgound;

        private ICommandImageLayer? _commandLayer;
        public IList<IImageLayer> LayerItems { get; private set; } = [];

        public SKColor? Backgound { get; set; }


        public event SelectionChangedEventHandler? SelectionChanged;

        public IImageLayer? this[int id] => Get<IImageLayer>(id);


        public T? Get<T>(int id)
            where T: IImageLayer
        {
            foreach (var item in LayerItems)
            {
                if (item.Id == id)
                {
                    return (T)item;
                }
            }
            return default;
        }

        public BitmapImageLayer AddImage(string fileName)
        {
            return Add(new BitmapImageLayer(fileName, this));
        }

        public async Task<BitmapImageLayer> AddImageAsync(IStorageFile file)
        {
            var fs = await file.OpenStreamForReadAsync();
            return Add(new BitmapImageLayer(SKBitmap.Decode(fs), this));
        }

        public void Clear()
        {
            LayerItems.Clear();
        }

        public TextImageLayer AddText(string text)
        {
            return Add(new TextImageLayer(text, this));
        }
        public void Add(IEnumerable<IImageLayer> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
        public T Add<T>(T layer)
            where T : IImageLayer
        {
            if (LayerItems.Contains(layer))
            {
                return layer;
            }
            if (LayerItems.Count > 0)
            {
                layer.Depth = LayerItems.MaxBy(item => item.Depth)!.Depth + 1;
            }
            layer.Id = ++_idGenerator;
            LayerItems.Add(layer);
            return layer;
        }

        public void Remove(int id)
        {
            for (var i = LayerItems.Count - 1; i >= 0; i--)
            {
                if (LayerItems[i].Id == id)
                {
                    LayerItems.RemoveAt(i);
                }
            }
        }

        public void Remove(IImageLayer layer)
        {
            LayerItems.Remove(layer);
        }

        /// <summary>
        /// 自动变更尺寸，显示全部内容
        /// </summary>
        public void Resize()
        {
            var outerWidth = 0;
            var outerHeight = 0;
            foreach (var item in LayerItems)
            {
                outerWidth = Math.Max(outerWidth, item.X + item.Width);
                outerHeight = Math.Max(outerHeight, item.Y + item.Height);
            }
            Resize(outerWidth, outerHeight);
        }
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            _transparentBackgound?.Invalidate();
            _surface?.Dispose();
            _surface = null;
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Tap(float x,  float y)
        {
            var items = LayerItems.Where(item => item.Visible).OrderByDescending(item => item.Depth);
            foreach (var item in items)
            {
                var offsetX = x - item.X;
                if (offsetX < 0 || offsetX > item.Width)
                {
                    continue;
                }
                var offsetY = y - item.Y;
                if (offsetY < 0 || offsetY > item.Height)
                {
                    continue;
                }
                Select(item);
                SelectionChanged?.Invoke(item.Id);
                return;
            }
        }
        public void Select(int id)
        {
            var layer = Get<IImageLayer>(id);
            if (layer is not null)
            {
                Select(layer);
            }
        }
        public void Select(IImageLayer layer)
        {
            _commandLayer ??= new SelectionImageLayer(this);
            _commandLayer.Resize(layer);
            Invalidate();
        }


        private void RenderSurface()
        {
            _surface ??= SKSurface.Create(new SKImageInfo(Width, Height));
            var canvas = _surface.Canvas;
            canvas.Clear(Backgound ?? SKColors.Transparent);
            if (Backgound is null)
            {
                _transparentBackgound ??= new TransparentImageLayer(this);
                _transparentBackgound.Paint(canvas);
            }
            foreach (var item in LayerItems)
            {
                item.Paint(canvas);
            }
            _commandLayer?.Paint(canvas);
        }

        public void Paint(SKCanvas canvas, SKImageInfo info)
        {
            RenderSurface();
            canvas.Clear(SKColors.Transparent);
            canvas.DrawSurface(_surface, 0, 0);
        }

        public void Invalidate()
        {
            InvalidateFn?.Invoke();
        }

        public void RegisterInvalidate(Action fn)
        {
            InvalidateFn = fn;
        }

        public void Dispose()
        {
            foreach (var item in LayerItems)
            {
                item.Dispose();
            }
            _transparentBackgound?.Dispose();
        }
    }
}
