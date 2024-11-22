using System;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageLayer: IDisposable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsVisible { get; set; }

        public bool IsLocked {  get; set; }

        public int Depth { get; set; }

        public IImageSource Source { get; }

        public IImageLayerTree Children { get; }
        /// <summary>
        /// 判断子节点是否启用
        /// </summary>
        public bool IsChildrenEnabled { get; }

        public IImageLayer? Get(Func<IImageLayer, bool> checkFn);

        public void Paint(IImageCanvas canvas);
    }
}
