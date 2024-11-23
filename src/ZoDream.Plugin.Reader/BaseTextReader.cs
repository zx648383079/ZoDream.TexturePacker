using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Readers
{
    public abstract class BaseTextReader : BaseTextReader<SpriteLayerSection>, ITextReader, IPluginReader
    {
    }
}
