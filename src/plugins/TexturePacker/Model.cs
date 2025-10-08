using System;
using System.Collections.Generic;

namespace ZoDream.Plugin.TexturePacker
{
    internal class TP_FrameRoot
    {
        public TP_FrameElement[] Frames { get; set; }

        public TP_Meta Meta { get; set; }
    }

    internal class TP_FrameRoot2
    {
        public IDictionary<string, TP_FrameElement2> Frames { get; set; }

        public TP_Meta Meta { get; set; }
    }

    internal class TP_FrameElement2
    {
        public TP_Bound Frame { get; set; }

        public TP_Size SourceSize { get; set; }
    }

    internal class TP_FrameElement
    {
        public string Filename { get; set; }

        public TP_Bound Frame { get; set; }

        public bool Rotated { get; set; }

        public bool Trimmed { get; set; }

        public TP_Bound SpriteSourceSize { get; set; }

        public TP_Size SourceSize { get; set; }
    }

    internal class TP_Bound : TP_Size
    {
        public int X { get; set; }

        public int Y { get; set; }

    }

    internal class TP_Size
    {
        public int W { get; set; }

        public int H { get; set; }
    }

    internal class TP_Meta
    {
        public Uri App { get; set; }

        public string Version { get; set; }

        public string Image { get; set; }

        public string Format { get; set; }

        public TP_Size Size { get; set; }

        public string Scale { get; set; }

        public string Smartupdate { get; set; }
    }
}
