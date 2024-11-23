using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public class ProjectDocument
    {

        public int Width { get; set; }

        public int Height { get; set; }

        public string? BackgroundColor { get; set; }

        public List<ImageLayer> LayerItems { get; set; } = [];

        public List<ImageResource> ResourceItems { get; set; } = [];
    }

}
