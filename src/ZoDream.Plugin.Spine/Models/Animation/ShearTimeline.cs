using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal class ShearTimeline: TranslateTimeline
    {
        public ShearTimeline(int frameCount) : base(frameCount)
        {
            
        }
        public override int PropertyId => ((int)TimelineType.Shear << 24) + BoneIndex;
    }
}
