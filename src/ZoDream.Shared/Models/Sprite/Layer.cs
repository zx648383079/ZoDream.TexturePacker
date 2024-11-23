using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.Models
{
    public class SpriteLayer: IImageBound
    {
        public string Name { get; set; } = string.Empty;

        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public int Rotate { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }

}
