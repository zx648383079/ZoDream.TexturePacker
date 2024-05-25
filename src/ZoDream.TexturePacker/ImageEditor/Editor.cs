using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class Editor (int width, int height) : IDisposable
    {
        private int Width = width;
        private int Height = height;
        private Action? InvalidateFn;
        private TransparentImageLayer? _transparentBackgound;
        public IList<IImageLayer> LayerItems { get; private set; } = [];

        public SKColor? Backgound { get; set; }

        public void AddImage(string fileName)
        {
            LayerItems.Add(new BitmapImageLayer(fileName));
        }

        public void Clear()
        {
            LayerItems.Clear();
        }

        public void AddText(string text)
        {
            LayerItems.Add(new TextImageLayer(text));
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            _transparentBackgound?.Dispose();
            _transparentBackgound = null;
        }

        public void Paint(SKCanvas canvas, SKImageInfo info)
        {
            canvas.Clear(Backgound ?? SKColors.Transparent);
            if (Backgound is null)
            {
                _transparentBackgound ??= new TransparentImageLayer(Width, Height);
                _transparentBackgound.Paint(canvas, info);
            }
            foreach (var item in LayerItems)
            {
                item.Paint(canvas, info);
            }
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
