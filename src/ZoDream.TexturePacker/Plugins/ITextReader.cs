using System.Collections.Generic;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins
{
    public interface ITextReader
    {
        public bool Canable(string content);
        public IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName);
        public string Serialize(IEnumerable<SpriteLayerSection> data, string fileName);
    }
}
