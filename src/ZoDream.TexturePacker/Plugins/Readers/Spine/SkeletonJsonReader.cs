using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Spine
{
    public class SkeletonJsonReader : BaseTextReader
    {
        public override bool Canable(string content)
        {
            return content.Contains("\"skeleton\"") && content.Contains("\"spine\"");
        }
        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
