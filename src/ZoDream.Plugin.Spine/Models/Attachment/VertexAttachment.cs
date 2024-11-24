namespace ZoDream.Plugin.Spine.Models
{
    internal class VertexAttachment: AttachmentBase
    {
        public int Id { get; set; }
        public int[] Bones { get; set; }
        public float[] Vertices { get; set; }
        public int WorldVerticesLength { get; set; }
    }
}
