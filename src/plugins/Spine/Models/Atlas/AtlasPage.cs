using System.Collections.Generic;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Spine.Models
{
    public class AtlasPage : ISpriteSection
    {
        public string Name { get; set; }
        public AtlasFormat Format { get; set; }
        public TextureFilter MinFilter { get; set; }
        public TextureFilter MagFilter { get; set; }
        public TextureWrap UWrap { get; set; } = TextureWrap.ClampToEdge;
        public TextureWrap VWrap { get; set; } = TextureWrap.ClampToEdge;
        public float Width { get; set; }
        public float Height { get; set; }

        public IList<ISpriteLayer> Items { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
    }
}
