using System.Collections.Generic;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.Models
{
    public class SpriteLayerSection: IImageSize
    {
        public bool UseCustomName { get; set; }
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public float Width { get; set; }
        public float Height { get; set; }

        public IList<SpriteLayer> Items { get; set; } = [];
        
    }
}
