using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.TexturePacker
{
    public class PlistReader : IPluginReader, ITextReader
    {
        public bool Canable(string content)
        {
            return content.Contains("<string>$TexturePacker:SmartUpdate:");
        }

        public LayerGroupItem? Deserialize(string content)
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
            var res = new LayerGroupItem();
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
            return res;
        }

        private LayerItem? ParseLayer(XmlNode? node)
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

        public async Task<LayerGroupItem?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text);
        }

        public async Task<LayerGroupItem?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text);
        }

        public string Serialize(LayerGroupItem data)
        {
            throw new NotImplementedException();
        }

        public async Task WriteAsync(string fileName, LayerGroupItem data)
        {
            await LocationStorage.WriteAsync(fileName, Serialize(data));
        }

        public async Task WriteAsync(IStorageFile file, LayerGroupItem data)
        {
            await FileIO.WriteTextAsync(file, Serialize(data), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
