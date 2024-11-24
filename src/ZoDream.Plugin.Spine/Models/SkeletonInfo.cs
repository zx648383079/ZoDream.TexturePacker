using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal class SkeletonInfo
    {
        public string Hash { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public string ImagesPath { get; set; }

        public string Version { get; set; }
        public float Fps { get; set; }
    }
}
