﻿using SkiaSharp;

namespace ZoDream.Plugin.Spine.Models
{
    internal class Slot
    {
        public string Attachment { get; set; }

        public string Bone { get; set; }

        public string Name { get; set; }

        public int Index { get; set; }
        public SKColor? Color { get; set; }
        public SKColor? DarkColor { get; set; }

        public BlendMode BlendMode { get; set; }
    }
}
