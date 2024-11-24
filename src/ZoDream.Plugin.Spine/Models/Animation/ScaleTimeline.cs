using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal class ScaleTimeline: TranslateTimeline
    {
        public ScaleTimeline(int frameCount): base(frameCount)
        {
            
        }
        public override int PropertyId => ((int)TimelineType.Scale<< 24) + BoneIndex;
    }
}
