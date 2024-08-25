using System.Collections.Generic;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins
{
    public interface ITextReader: ITextReader<IEnumerable<SpriteLayerSection>>
    {
    }
}
