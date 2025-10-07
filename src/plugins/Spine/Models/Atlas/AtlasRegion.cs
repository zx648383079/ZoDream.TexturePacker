using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Spine.Models
{
    public class AtlasRegion : TextureRegion, ISpriteLayer
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public int Index { get; set; }
        public float Rotate { get; set; }
        public int[] Splits { get; set; }
        public int[] Pads { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ShearX { get; set; }
        public float ShearY { get; set; }
    }
}
