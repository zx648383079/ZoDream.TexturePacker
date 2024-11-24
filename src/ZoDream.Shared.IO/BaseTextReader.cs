using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.IO
{
    public abstract class BaseTextReader : BaseTextReader<SpriteLayerSection>, ITextReader, IPluginReader
    {
    }
}
