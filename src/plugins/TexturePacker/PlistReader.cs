using System;
using System.Collections.Generic;
using System.Xml;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.TexturePacker
{
    public class PlistReader : BaseTextReader
    {
        public override bool IsEnabled(string content)
        {
            return content.Contains("<string>$TexturePacker:SmartUpdate:");
        }

        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            var doc = new XmlDocument();
            doc.LoadXml(content);
            var root = doc.DocumentElement?.FirstChild;
            if (root is null || root.Name != "dict")
            {
                return null;
            }
            var frames = GetNodeByKey(root, "frames");
            if (frames == null)
            {
                return null;
            }
            var res = new SpriteLayerSection();
            var i = 0;
            while (i < frames.ChildNodes.Count)
            {
                var node = frames.ChildNodes[i];
                if (node.Name == "key")
                {
                    var layer = ParseLayer(frames.ChildNodes[i + 1]);
                    if (layer is not null)
                    {
                        layer.Name = node.InnerText;
                        res.Items.Add(layer);
                    }
                    i++;
                }
                i++;
            }
            return [res];
        }

        private SpriteLayer? ParseLayer(XmlNode? node)
        {
            if (node == null)
            {
                return null;
            }
            var frame = GetNodeByKey(node, "frame");
            var rotatedKey = "rotated";
            if (frame is null)
            {
                frame = GetNodeByKey(node, "textureRect");
                rotatedKey = "textureRotated";
            }
            var (x, y, w, h) = ParseRect(frame?.InnerText);
            var rotated = GetNodeByKey(node, rotatedKey)?.Name == "true";
            if (rotated)
            {
                (w, h) = (h, w);
            }
            return new()
            {
                Rotate = rotated ? 90: 0,
                X = x,
                Y = y,
                Width = w,
                Height = h
            };
        }

        private (int,int) ParseSize(string text)
        {
            var args = text[1..^1].Split(',');
            return (int.Parse(args[0]), int.Parse(args[1]));
        }
        private (int, int, int, int) ParseRect(string text)
        {
            var args = text.Replace("{", string.Empty)
                .Replace("}", string.Empty).Split(',');
            return (int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]));
        }

        private XmlNode? GetNodeByKey(XmlNode node, string key)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var item = node.ChildNodes[i];
                if (item is null)
                {
                    continue;
                }
                if (item.Name == "key" && item.InnerText == key)
                {
                    return node.ChildNodes[i + 1];
                }
            }
            return null;
        }


        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            throw new NotImplementedException();
        }

    }
}
