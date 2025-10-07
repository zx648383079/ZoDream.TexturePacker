using System.Collections.Generic;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.Interfaces
{
    public interface ISpriteLayer : IImageBound, IImageStyle, IReadOnlyStyle
    {
        public string Name { get; set; }

        public new float X { get; set; }
        public new float Y { get; set; }
        public new float Rotate { get; set; }

        public new float ScaleX { get; set; }
        public new float ScaleY { get; set; }
        /// <summary>
        /// 错切 相当 skewX
        /// </summary>
        public new float ShearX { get; set; }
        public new float ShearY { get; set; }
    }

    public interface ISpriteSection : IImageSize
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public IList<ISpriteLayer> Items { get; set; }
    }
}
