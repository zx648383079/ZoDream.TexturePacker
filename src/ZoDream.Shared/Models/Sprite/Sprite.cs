using System.Collections.Generic;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Models
{
    public class SpriteLayerSection: ISpriteSection
    {
        public bool UseCustomName { get; set; }
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public float Width { get; set; }
        public float Height { get; set; }

        public IList<ISpriteLayer> Items { get; set; } = [];
        
    }
}
