using SkiaSharp;

namespace ZoDream.Plugin.Spine.Models
{
    internal class MeshAttachment: VertexAttachment
    {
        public int HullLength { get; set; }
        public float[] RegionUVs { get; set; }
        public float[] UVs { get; set; }
        public int[] Triangles { get; set; }

        public SKColor? Color { get; set; }

        public string Path { get; set; }
        public object RendererObject { get; set; }
        public float RegionU { get; set; }
        public float RegionV { get; set; }
        public float RegionU2 { get; set; }
        public float RegionV2 { get; set; }
        public bool RegionRotate { get; set; }
        public float RegionOffsetX { get; set; }
        public float RegionOffsetY { get; set; }
        public float RegionWidth { get; set; }
        public float RegionHeight { get; set; }
        public float RegionOriginalWidth { get; set; }
        public float RegionOriginalHeight { get; set; }

        public bool InheritDeform { get; set; }

        public int[] Edges { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Sequence? Sequence { get; internal set; }
    }
}
