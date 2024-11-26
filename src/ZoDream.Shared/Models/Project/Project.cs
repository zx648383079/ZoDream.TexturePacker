using System.Collections.Generic;
using ZoDream.Shared.Drawing;

namespace ZoDream.Shared.Models
{
    public class ProjectDocument
    {

        public int Width { get; set; }

        public int Height { get; set; }

        public string? BackgroundColor { get; set; }

        public List<ProjectImageLayer> LayerItems { get; set; } = [];

        public List<IImageData> ResourceItems { get; set; } = [];

        public List<SpriteLayerSection> SpriteItems { get; set; } = [];
        public List<SkeletonSection> SkeletonItems { get; set; } = [];


    }

}
