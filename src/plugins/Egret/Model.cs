using System.Collections.Generic;

namespace ZoDream.Plugin.Egret
{
    internal class ER_FrameSheetFile
    {
        public string File { get; set; } = string.Empty;

        public IDictionary<string, ER_FrameItem>? Frames { get; set; }

    }

    internal class ER_FrameItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public int OffX { get; set; }
        public int OffY { get; set; }
        public int SourceH { get; set; }
        public int SourceW { get; set; }
    }

    internal class ER_JsonFile
    {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public IList<ER_JsonSubItem>? SubTexture { get; set; }

    }

    internal class ER_JsonSubItem
    {
        public string Name { get; set; } = string.Empty;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameX { get; set; }
        public int FrameY { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
    }
}
