using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private Action? InvalidateFn;

        private SKSurface? _surface;

        private TransparentImageLayer? _transparentBackgound;

        private ICommandImageLayer? _commandLayer;
        public IList<IImageLayer> LayerItems { get; private set; } = [];

        public SKColor? Backgound { get; set; }

        public void AddImage(string fileName)
        {
            Add(new BitmapImageLayer(fileName, this));
        }

        public void Clear()
        {
            LayerItems.Clear();
        }

        public void AddText(string text)
        {
            Add(new TextImageLayer(text, this));
        }

        public void Add(IImageLayer layer)
        {
            if (LayerItems.Contains(layer))
            {
                return;
            }
            if (LayerItems.Count > 0)
            {
                layer.Depth = LayerItems.MaxBy(item => item.Depth)!.Depth + 1;
            }
            LayerItems.Add(layer);
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
                return;
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
