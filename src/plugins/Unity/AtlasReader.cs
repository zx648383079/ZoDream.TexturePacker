using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Unity
{
    public class AtlasReader : BaseTextReader
    {
        public override bool IsEnabled(string content)
        {
            return content.Contains("bounds:") && content.Contains("offsets:");
        }

        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            var res = new SpriteLayerSection()
            {
                UseCustomName = true
            };
            var last = new SpriteLayer();
            var isLayer = false;
            var lines = content.Split('\n').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (!line.Contains(':'))
                {
                    if (i > 1)
                    {
                        isLayer = true;
                        last = new SpriteLayer()
                        {
                            Name = line,
                        };
                        res.Items.Add(last);
                    }
                    else
                    {
                        res.Name = line[0..line.LastIndexOf('.')];
                    }
                }
                var args = line.Trim().Split(':', 2);
                switch (args[0])
                {
                    case "size":
                        var (w, h) = TryParse(args[1]);
                        if (!isLayer)
                        {
                            res.Width = w;
                            res.Height = h;
                        }
                        break;
                    case "bounds":
                        if (isLayer)
                        {
                            var items = args[1].Split(',');
                            last.X = int.Parse(items[0]);
                            last.Y = int.Parse(items[1]);
                            last.Width = int.Parse(items[2]);
                            last.Height = int.Parse(items[3]);
                        }
                        break;
                    default:
                        break;
                }
            }
            return [res];
        }

        private (int, int) TryParse(string text)
        {
            var args = text.Split(',');
            return (int.Parse(args[0].Trim()), int.Parse(args[1].Trim()));
        }

        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            foreach (var res in data)
            {
                var sb = new StringBuilder();
                sb.AppendLine(res.Name)
                    .AppendLine($"size: {res.Width},{res.Height}")
                    .AppendLine("filter: Linear,Linear");
                foreach (var item in res.Items)
                {
                    sb.AppendLine(item.Name)
                        .AppendLine($"bounds:{item.X},{item.Y},{item.Width},{item.Height}")
                        .AppendLine($"offset:6,6,{item.Width + 12},{item.Height + 12}")
                        ;
                }
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}
