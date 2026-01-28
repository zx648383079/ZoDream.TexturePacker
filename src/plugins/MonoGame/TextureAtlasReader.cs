using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.MonoGame
{
    public class TextureAtlasReader : BaseTextReader
    {
        public override bool IsEnabled(string content)
        {
            return content.Contains("</TextureAtlas>");
        }

        public override IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            var res = new SpriteLayerSection();
            using var reader = XmlReader.Create(new StringReader(content));
            var doc = XDocument.Load(reader);
            var root = doc.Root;
            res.Name = res.FileName = root.Element("Texture").Value;
            var regions = root.Element("Regions")?.Elements("Region");

            if (regions != null)
            {
                foreach (var region in regions)
                {
                    var name = region.Attribute("name")?.Value;
                    int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                    int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                    int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                    int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                    res.Items.Add(new SpriteLayer()
                    {
                        Name = name ?? string.Empty,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height
                    });
                }
            }

            return [res];
        }

        public override string Serialize(IEnumerable<ISpriteSection> data, string fileName)
        {
            var sb = new StringBuilder();
            using var writer = XmlWriter.Create(sb);
            foreach (var item in data)
            {
                writer.WriteStartDocument(false);
                writer.WriteStartElement("TextureAtlas");
                writer.WriteStartElement("Texture");
                writer.WriteString(item.Name ?? Path.GetFileNameWithoutExtension(fileName));
                writer.WriteEndElement();

                writer.WriteStartElement("Regions");

                foreach (var layer in item.Items)
                {
                    writer.WriteStartElement("Region");
                    writer.WriteAttributeString("name", layer.Name);
                    writer.WriteAttributeString("x", layer.X.ToString());
                    writer.WriteAttributeString("y", layer.Y.ToString());
                    writer.WriteAttributeString("width", layer.Width.ToString());
                    writer.WriteAttributeString("height", layer.Height.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            return sb.ToString();
        }
    }
}
