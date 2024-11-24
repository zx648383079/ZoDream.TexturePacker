namespace ZoDream.Plugin.Spine.Models
{
    internal class PathAttachment: VertexAttachment
    {
        public float[] Lengths { get; set; }
        public bool Closed { get; set; }
        public bool ConstantSpeed { get; set; }
    }
}
