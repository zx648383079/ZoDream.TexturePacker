using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal class PathConstraintSpacingTimeline: PathConstraintPositionTimeline
    {
        public PathConstraintSpacingTimeline(int frameCount): base(frameCount)
        {
            
        }
        public override int PropertyId => ((int)TimelineType.PathConstraintSpacing << 24) + PathConstraintIndex;
    }
}
