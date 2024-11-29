using System;

namespace ZoDream.Shared.EditorInterface
{
    public interface IImageLayer: IDisposable
    {

        public IImageLayer? Parent { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsVisible { get; set; }

        public bool IsLocked {  get; set; }

        public int Depth { get; set; }

        public IImageSource Source { get; set; }

        public IImageLayerTree Children { get; }
        /// <summary>
        /// 判断子节点是否启用
        /// </summary>
        public bool IsChildrenEnabled { get; }

        public IImageLayer? Get(Func<IImageLayer, bool> checkFn);
        /// <summary>
        /// 源更新了，需要重采样
        /// </summary>
        public void Resample();

        public void Paint(IImageCanvas canvas);
    }
}
