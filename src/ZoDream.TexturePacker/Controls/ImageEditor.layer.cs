using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.ImageEditor;

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
        private TransparentImageLayer? _transparentBackgound;

        private ICommandImageLayer? _commandLayer;
        public IList<IImageLayer> LayerItems { get; private set; } = [];

        public SKColor? Backgound { get; set; }


        public int ActualHeightI => _heightI;
        public int ActualWidthI => _widthI;

        public IImageLayer? this[int id] => Get<IImageLayer>(id);


        public T? Get<T>(int id)
            where T : IImageLayer
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
            GenerateLayerId(layer);
            LayerItems.Add(layer);
            return layer;
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
            _widthI = width; 
            _heightI = height;
            ResizeWithControl(width, height);
            _transparentBackgound?.Invalidate();
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
                SelectedCommand?.Execute(item.Id);
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
            canvas.Clear(Backgound ?? SKColors.Transparent);
            if (Backgound is null)
            {
                _transparentBackgound ??= new TransparentImageLayer(this);
                _transparentBackgound.Paint(canvas);
            }
            foreach (var item in LayerItems)
            {
                if (!item.Visible)
                {
                    continue;
                }
                item.Paint(canvas);
            }
            _commandLayer?.Paint(canvas);
        }

        public void Invalidate()
        {
            CanvasTarget.Invalidate();
        }

        public void SaveAs(string fileName)
        {
            using var bitmap = new SKBitmap(ActualWidthI, ActualHeightI);
            using var canvas = new SKCanvas(bitmap);
            foreach (var item in LayerItems)
            {
                if (!item.Visible)
                {
                    continue;
                }
                item.Paint(canvas);
            }
            canvas.Flush();
            using var fs = File.OpenWrite(fileName);
            bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
        }

        public void SaveAs(IImageLayer layer, string fileName)
        {
            if (layer is null)
            {
                return;
            }
            var x = layer.X;
            var y = layer.Y;
            layer.X = 0;
            layer.Y = 0;
            using var bitmap = layer.PaintRotate(-layer.RotateDeg);
            using var fs = File.OpenWrite(fileName);
            bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
            layer.X = x;
            layer.Y = y;
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
