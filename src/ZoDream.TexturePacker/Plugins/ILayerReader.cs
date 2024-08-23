using System.Collections.Generic;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins
{
    public interface IPluginReader: IPluginReader<IEnumerable<SpriteLayerSection>>
    {
    }
}
