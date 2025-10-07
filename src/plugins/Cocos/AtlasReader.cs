using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Cocos
{
    public class AtlasReader : BaseTextReader
    {
        public override bool IsEnabled(string content)
        {
            return content.Contains("xy:") && content.Contains("format:");
        }

        public override IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            var res = new SpriteLayerSection();
            var last = new SpriteLayer();
            var lines = content.Split('\n').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!line.Contains(':'))
                {
                    if (lines[i + 1].StartsWith(' '))
                    {
                        last = new SpriteLayer()
                        {
                            Name = line,
                        };
                        res.Items.Add(last);
                    }
                    else
                    {
                        res.Name = line;
                    }
                }
                var isLayer = line.StartsWith(' ');
                var args = line.Trim().Split(':', 2);
                switch (args[0])
                {
                    case "size":
                        var (w, h) = TryParse(args[1]);
                        if (isLayer)
                        {
                            if (IsRotate90(last.Rotate))
                            {
                                (w, h) = (h, w);
                            }
                            last.Width = w;
                            last.Height = h;

                        }
                        else
                        {
                            res.Width = w;
                            res.Height = h;
                        }
                        break;
                    case "xy":
                        var (x, y) = TryParse(args[1]);
                        if (isLayer)
                        {
                            last.X = x;
                            last.Y = y;
                        }
                        break;
                    case "rotate":
                        if (isLayer)
                        {
                            last.Rotate = TryParseRotate(args[1].Trim());
                            if (IsRotate90(last.Rotate) && last.Width > 0)
                            {
                                (last.Width, last.Height) = (last.Height, last.Width);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return [res];
        }


        private bool IsRotate90(float deg)
        {
            return Math.Abs(deg) % 180 == 90;
        }

        private int TryParseRotate(string text)
        {
            if (text == "true")
            {
                return 90;
            }
            if (text == "false")
            {
                return 0;
            }
            if (int.TryParse(text, out var res))
            {
                return res;
            }
            return 0;
        }

        private (int, int) TryParse(string text)
        {
            var args = text.Split(',');
            return (int.Parse(args[0].Trim()), int.Parse(args[1].Trim()));
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
