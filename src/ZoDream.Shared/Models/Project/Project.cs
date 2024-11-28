using System.Collections.Generic;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.Models
{
    public class ProjectDocument: IImageSize
    {

        public float Width { get; set; }

        public float Height { get; set; }

        public string? BackgroundColor { get; set; }

        public List<ProjectImageLayer> LayerItems { get; set; } = [];

        public List<IImageData> ResourceItems { get; set; } = [];

        public List<SpriteLayerSection> SpriteItems { get; set; } = [];
        public List<SkeletonSection> SkeletonItems { get; set; } = [];


    }

}
