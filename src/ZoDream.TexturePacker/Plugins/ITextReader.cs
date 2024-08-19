using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins
{
    public interface ITextReader
    {
        public bool Canable(string content);
        public SpriteLayerSection? Deserialize(string content);
        public string Serialize(SpriteLayerSection data);
    }
}
