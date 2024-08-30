using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Plugins;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageEditor: IDisposable
    {

        public int ActualHeightI { get; }
        public int ActualWidthI { get; }

        public SKColor? Backgound { get; set; }

        public IList<IImageLayer> LayerItems { get; }

        public void GenerateLayerId(IImageLayer layer);

        public IImageLayer? this[int id] { get; }

        public T? Get<T>(int id) where T : IImageLayer;

        public Task<IImageLayer?> AddImageAsync(string fileName);

        public Task<IImageLayer?> AddImageAsync(IStorageFile file);
        public IImageLayer? AddImage(SKBitmap? image);
        public IImageLayer? AddImage(IImageData? image);

        public TextImageLayer AddText(string text);

        public void Add(IEnumerable<IImageLayer> items);

        public T Add<T>(T layer) where T : IImageLayer;

        public void Clear();

        public void Remove(int id);

        public void Remove(IImageLayer layer);

        public void Resize();

        public void Resize(int width, int height);

        public void Tap(float x, float y);

        public void Select(int id);

        public void Select(IImageLayer layer);

        public void Unselect();

        public void Paint(SKCanvas canvas, SKImageInfo info);

        public void Invalidate();

        public void SaveAs(string fileName);

        public void SaveAs(IImageLayer layer, string fileName);


    }
}
