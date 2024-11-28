using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.Models
{
    public class SpriteLayer: IImageBound
    {
        public string Name { get; set; } = string.Empty;

        public float X { get; set; }

        public float Y { get; set; }

        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public float Rotate { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }
    }

}
