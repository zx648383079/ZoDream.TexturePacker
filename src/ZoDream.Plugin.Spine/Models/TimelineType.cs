using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal enum TimelineType
    {
        Rotate = 0, Translate, Scale, Shear, //
        Attachment, Color, Deform, //
        Event, DrawOrder, //
        IkConstraint, TransformConstraint, //
        PathConstraintPosition, PathConstraintSpacing, PathConstraintMix, //
        TwoColor
    }
}
