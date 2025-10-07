using System.Numerics;

namespace ZoDream.Plugin.Spine.Models
{
    public class Sequence
    {
        public int Start { get; set; }
        public int Digits { get; set; }
        public int SetupIndex { get; set; }
        public TextureRegion[] Regions { get; set; }

        public Sequence(int count)
        {
            Regions = new TextureRegion[count];
        }

        public void Apply(Slot slot, IHasTextureRegion attachment)
        {
            var index = slot.Runtime.SequenceIndex;
            if (index == -1)
            {
                index = SetupIndex;
            }
            if (index >= Regions.Length)
            {
                index = Regions.Length - 1;
            }
            TextureRegion region = Regions[index];
            if (attachment.Region != region)
            {
                attachment.Region = region;
                attachment.UpdateRegion();
            }
        }
    }

    public class TextureRegion
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Uv { get; set; }
        public Vector2 Uv2 { get; set; }
    }
}
