using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Spine
{
    public partial class AtlasReader : BaseTextReader
    {
        public override bool IsEnabled(string content)
        {
            if (content.Contains("bounds:") && content.Contains("offsets:")) 
            {
                return true; 
            }
            return content.Contains("xy:") && content.Contains("format:");
        }

        public override IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            using var reader = new StringReader(content);
            foreach (var item in Deserialize(reader))
            {
                yield return item;
            }
        }


        

        public override string Serialize(IEnumerable<ISpriteSection> data, string fileName)
        {
            foreach (var res in data)
            {
                var sb = new StringBuilder();
                sb.AppendLine(res.Name)
                    .AppendLine($"size: {res.Width},{res.Height}")
                    .AppendLine("format: RGBA8888")
                    .AppendLine("filter: Linear,Linear")
                    .AppendLine("repeat: none");
                foreach (var item in res.Items)
                {
                    sb.AppendLine(item.Name)
                        .AppendLine("  rotate: " + (item.Rotate == 90 ? "true" : "false"))
                        .AppendLine($"  xy: {item.X}, {item.Y}")
                        .AppendLine($"  size: {item.Width}, {item.Height}")
                        .AppendLine($"  orig: {item.Width}, {item.Height}")
                        .AppendLine("  offset: 0, 0")
                        .AppendLine("  index: -1")
                        ;
                }
                return sb.ToString();
            }
            return string.Empty;
        }

    }
}
