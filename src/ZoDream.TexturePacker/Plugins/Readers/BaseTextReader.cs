using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public abstract class BaseTextReader : BaseTextReader<SpriteLayerSection>, ITextReader, IPluginReader
    {
    }
}
