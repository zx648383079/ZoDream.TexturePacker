using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class FolderImageLayer(IImageEditor editor) : BaseImageLayer(editor)
    {
        public IList<IImageLayer> Children { get; private set; } = [];

        private SKSurface? _surface;

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
            if (Children.Contains(layer))
            {
                return layer;
            }
            if (Children.Count > 0)
            {
                layer.Depth = Children.MaxBy(item => item.Depth)!.Depth + 1;
            }
            Editor.GenerateLayerId(layer);
            Children.Add(layer);
            return layer;
        }

        public void Remove(int id)
        {
            for (var i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i].Id == id)
                {
                    Children.RemoveAt(i);
                }
            }
        }

        public void Remove(IImageLayer layer)
        {
            Children.Remove(layer);
        }

        private void Resize()
        {
            var outerWidth = 0;
            var outerHeight = 0;
            foreach (var item in Children)
            {
                outerWidth = Math.Max(outerWidth, item.X + item.Width);
                outerHeight = Math.Max(outerHeight, item.Y + item.Height);
            }
            Width = outerWidth;
            Height = outerHeight;
        }

        private void RenderSurface()
        {
            Resize();
            var info = new SKImageInfo(Width, Height);
            _surface = SKSurface.Create(info);
            var canvas = _surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            foreach (var item in Children)
            {
                if (!item.Visible)
                {
                    continue;
                }
                item.Paint(canvas);
            }
        }

        public override void Paint(SKCanvas canvas)
        {
            if (!Visible)
            {
                return;
            }
            if (_surface == null)
            {
                RenderSurface();
            }
            canvas.DrawSurface(_surface, X, Y);
        }
    }
}
