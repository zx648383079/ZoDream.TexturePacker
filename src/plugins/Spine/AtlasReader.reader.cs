using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using ZoDream.Plugin.Spine.Models;

namespace ZoDream.Plugin.Spine
{
    public partial class AtlasReader
    {

        internal IEnumerable<AtlasPage> Deserialize(TextReader reader)
        {
            AtlasPage? page = null;
            AtlasRegion? region = null;
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                if (line.Trim().Length == 0)
                {
                    if (page is not null)
                    {
                        UpdateRegion(page);
                        yield return page;
                        page = null;
                    }
                    continue;
                }
                if (page is null)
                {
                    page = new AtlasPage()
                    {
                        Name = line,
                    };
                    region = null;
                    continue;
                }
                if (!line.Contains(':'))
                {
                    region = new AtlasRegion()
                    {
                        Name = line,
                    };
                    page.Items.Add(region);
                    continue;
                }
                var isLayer = page.Items.Count > 0; // region is not null && (line.StartsWith(' ') || line.StartsWith('\t'));
                var args = line.Trim().Split(':', 2);
                switch (args[0].ToLower())
                {
                    case "size":
                        var (w, h) = TryParse(args[1]);
                        if (isLayer)
                        {
                            region.Width = w;
                            region.Height = h;

                        }
                        else
                        {
                            page.Width = w;
                            page.Height = h;
                        }
                        break;
                    case "orig":
                        if (isLayer)
                        {
                            (region.OriginalWidth, region.OriginalHeight) = TryParse(args[1]);
                        }
                        break;
                    case "xy":
                        if (isLayer)
                        {
                            (region.X, region.Y) = TryParse(args[1]);
                        }
                        break;
                    case "offset":
                        if (isLayer)
                        {
                            (region.OffsetX, region.OffsetY) = TryParse(args[1]);
                        }
                        break;
                    case "bounds":
                        if (isLayer)
                        {
                            var items = args[1].Split(',');
                            region.X = int.Parse(items[0]);
                            region.Y = int.Parse(items[1]);
                            region.Width = int.Parse(items[2]);
                            region.Height = int.Parse(items[3]);
                        }
                        break;
                    case "offsets":
                        if (isLayer)
                        {
                            var items = args[1].Split(',');
                            region.OffsetX = int.Parse(items[0]);
                            region.OffsetY = int.Parse(items[1]);
                            region.OriginalWidth = int.Parse(items[2]);
                            region.OriginalHeight = int.Parse(items[3]);
                        }
                        break;
                    case "index":
                        if (isLayer)
                        {
                            region.Index = int.Parse(args[1].Trim());
                        }
                        break;
                    case "rotate":
                        if (isLayer)
                        {
                            region.Rotate = TryParseRotate(args[1].Trim());
                        }
                        break;
                    case "format":
                        if (!isLayer)
                        {
                            page.Format = Enum.Parse<AtlasFormat>(args[1].Trim(), false);
                        }
                        break;
                    case "filter":
                        if (!isLayer)
                        {
                            var filters = args[1].Split(',');
                            page.MinFilter = Enum.Parse<TextureFilter>(filters[0].Trim(), false);
                            if (filters.Length > 1)
                            {
                                page.MagFilter = Enum.Parse<TextureFilter>(filters[1].Trim(), false);
                            }
                        }
                        break;
                    case "repeat":
                        if (!isLayer)
                        {
                            break;
                        }
                        switch (args[1].Trim())
                        {
                            case "x":
                                page.UWrap = TextureWrap.Repeat;
                                break;
                            case "y":
                                page.VWrap = TextureWrap.Repeat;
                                break;
                            case "xy":
                                page.UWrap = page.VWrap = TextureWrap.Repeat;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        if (isLayer)
                        {
                            var tuple = args[1].Split(",");
                            if (tuple.Length == 4)
                            {
                                if (region.Splits is not null && region.Splits.Length == 4)
                                {
                                    region.Pads = tuple.Select(i => int.Parse(i.Trim())).ToArray();
                                } else
                                {
                                    region.Splits = tuple.Select(i => int.Parse(i.Trim())).ToArray();
                                }
                            }
                        }
                    break;
                }
            }

            if (page is not null)
            {
                UpdateRegion(page);
                yield return page;
            }
        }

        private void UpdateRegion(AtlasPage page)
        {
            foreach (AtlasRegion region in page.Items)
            {
                if (region.OriginalWidth == 0 && region.OriginalHeight == 0)
                {
                    region.OriginalWidth = (int)region.Width;
                    region.OriginalHeight = (int)region.Height;
                }
                region.Uv = new Vector2(region.X / page.Width, region.Y / page.Height);
                if (region.Rotate == 90)
                {
                    region.Uv2 = new Vector2((region.X + region.Height) / page.Width, 
                        (region.Y + region.Width) / page.Height);

                    //int tempSwap = region.PackedWidth;
                    //region.PackedWidth = region.packedHeight;
                    //region.packedHeight = tempSwap;
                }
                else
                {
                    region.Uv2 = new Vector2((region.X + region.Width) / page.Width,
                        (region.Y + region.Height) / page.Height);
                }
            }
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

    }
}
