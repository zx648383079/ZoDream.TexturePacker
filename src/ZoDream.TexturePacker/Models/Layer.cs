using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.TexturePacker.Models
{
    public class LayerItem
    {
        public string Name { get; set; } = string.Empty;

        public int X { get; set; }

        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class LayerGroupItem
    {
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public int Width { get; set; }
        public int Height { get; set; }

        public IList<LayerItem> Items { get; set; } = [];
    }
}
