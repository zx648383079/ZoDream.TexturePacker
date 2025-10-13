using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.MonoGame
{
    public class MGCBReader : BaseTextReader
    {

        public override bool IsEnabled(string content)
        {
            return content.Contains("<TextureAtlas>") && content.Contains("<Regions>");
        }
        public override IEnumerable<ISpriteSection>? Deserialize(string content, string fileName)
        {
            var doc = XDocument.Parse(content);
            var root = doc.Root;
            var val = 0;
            var res = new SpriteLayerSection()
            {
                FileName = root.Element("Texture")?.Value,
                Items = root.Element("Regions")
                         ?.Elements("Region").Select(region => new SpriteLayer()
                         {
                             Name = region.Attribute("name")?.Value,
                             X = int.TryParse(region.Attribute("x")?.Value, out val) ? val : 0,
                             Y = int.TryParse(region.Attribute("y")?.Value, out val) ? val : 0,
                             Width = int.TryParse(region.Attribute("width")?.Value, out val) ? val : 0,
                             Height = int.TryParse(region.Attribute("height")?.Value, out val) ? val : 0,
                         }).ToArray() ?? []
            };
            yield return res;
        }

        public override string Serialize(IEnumerable<ISpriteSection> data, string fileName)
        {
            foreach (var item in data)
            {
                var regions = new XElement("Regions",
                item.Items.Select(region =>
                        new XElement("Region",
                            new XAttribute("name", region.Name ?? ""),
                            new XAttribute("x", region.X),
                            new XAttribute("y", region.Y),
                            new XAttribute("width", region.Width),
                            new XAttribute("height", region.Height)
                        )
                    )
                );

                var root = new XElement("TextureAtlas",
                    new XElement("Texture", item.FileName ?? ""),
                    regions
                );

                return new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    root
                ).ToString();
            }
            return string.Empty;
            //using var writer = XmlWriter.Create(fileName);
            //foreach (var item in data)
            //{
            //    writer.WriteStartDocument(true);
            //    writer.WriteStartElement("TextureAtlas");
            //    writer.WriteStartElement("Texture");
            //    writer.WriteString(item.FileName);
            //    writer.WriteEndElement();

            //    writer.WriteStartElement("Regions");
            //    foreach (var it in item.Items)
            //    {
            //        writer.WriteStartElement("Region");
            //        writer.WriteAttributeString("name", it.Name);
            //        writer.WriteAttributeString("x", it.X.ToString());
            //        writer.WriteAttributeString("y", it.Y.ToString());
            //        writer.WriteAttributeString("width", it.Width.ToString());
            //        writer.WriteAttributeString("height", it.Height.ToString());
            //        writer.WriteEndElement();
            //    }
            //    writer.WriteEndElement();

            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}
        }
    }
}
