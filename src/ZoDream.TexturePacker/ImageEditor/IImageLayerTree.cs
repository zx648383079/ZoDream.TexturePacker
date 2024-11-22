using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageLayerTree: IList<IImageLayer>
    {
        /// <summary>
        /// 添加到首
        /// </summary>
        /// <param name="layer"></param>
        public void AddFirst(IImageLayer layer);
        public void AddRange(IEnumerable<IImageLayer> items);
        public bool RemoveIfKid(IImageLayer layer);
        public IImageLayer? Get(int id);

        public IImageLayer? Get(Func<IImageLayer, bool> checkFn);

        public IImageLayer? Get(float x, float y);

        public void Paint(IImageCanvas canvas);
    }
}
