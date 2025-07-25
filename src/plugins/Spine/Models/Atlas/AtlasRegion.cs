namespace ZoDream.Plugin.Spine.Models
{
    public class AtlasRegion : TextureRegion
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public int Index { get; set; }
        public int Rotate { get; set; }
        public int[] Splits { get; set; }
        public int[] Pads { get; set; }
    }
}
