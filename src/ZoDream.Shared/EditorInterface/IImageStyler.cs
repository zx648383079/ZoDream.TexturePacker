using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ZoDream.Shared.EditorInterface
{
    public interface IImageStyler
    {
        public string Name { get; }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public IImageStyle Compute(IImageLayer layer);
    }

    public interface IImageComputedStyler: IImageStyler
    {
        public float ActualWidth { get; }
        public float ActualHeight { get; }
        /// <summary>
        /// 计算所有的样式
        /// </summary>
        /// <param name="items"></param>
        public void Compute(IImageLayerTree items);

        public void Paint(IImageLayerTree items, IImageCanvas canvas);
        public void Paint(IImageLayerTree items, SKCanvas canvas);
        /// <summary>
        /// 根据坐标获取图层
        /// </summary>
        /// <param name="items"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKPoint point);
        /// <summary>
        /// 根据选区获取图层
        /// </summary>
        /// <param name="items"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKRect rect);
        public void Clear();
        
    }

    public interface IImageStyleManager: IList<IImageStyler>, IDisposable
    {
        public bool TryGet(string name, [NotNullWhen(true)] out IImageStyler? styler);
    }
}
