using System;
using System.Collections.Generic;
using System.Linq;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Unity
{
    public class AssetReader : BaseTextReader
    {
        public override bool Canable(string content)
        {
            return content.Contains("Sprite:") && content.Contains("m_RD:");
        }
        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            var lines = content.Split('\n').Where(item => !string.IsNullOrWhiteSpace(item));
            var minCount = -1;
            var blockCount = -1;
            var res = new SpriteLayerSection()
            {
                UseCustomName = true,
                Items = [new()]
            };
            foreach (var line in lines)
            {
                var text = line.TrimStart();
                if (minCount < 0 && text.StartsWith("m_Name:")) 
                {
                    res.Items[0].Name = text.Split(':', 2)[1].Trim();
                }
                var whitespace = line.Length - text.Length;
                if (minCount > 0 && whitespace <= minCount)
                {
                    break;
                }
                if (blockCount > 0)
                {
                    if (blockCount >= whitespace)
                    {
                        blockCount = -1;
                    } else
                    {
                        var args = text.Split(':');
                        switch (args[0])
                        {
                            case "x":
                                res.Items[0].X = ReaderHelper.TryParseInt(args[1]); 
                                break;
                            case "y":
                                res.Items[0].Y = -ReaderHelper.TryParseInt(args[1]);
                                break;
                            case "width":
                                res.Items[0].Width = ReaderHelper.TryParseInt(args[1]);
                                break;
                            case "height":
                                res.Items[0].Height = ReaderHelper.TryParseInt(args[1]);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (minCount > 0 && text.StartsWith("texture:"))
                {
                    res.Name = ReaderHelper.MatchWithRange(text, "guid:", ",");
                }
                if (minCount > 0 && text.StartsWith("textureRect:"))
                {
                    blockCount = whitespace;
                }
                if (text == "m_RD:")
                {
                    minCount = whitespace;
                    continue;
                }
            }
            if (string.IsNullOrWhiteSpace(res.Name))
            {
                return null;
            }
            return [res];
        }


        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
