﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Spine.Models
{
    internal enum TransformMode
    {
        //0000 0 Flip Scale Rotation
        Normal = 0, // 0000
        OnlyTranslation = 7, // 0111
        NoRotationOrReflection = 1, // 0001
        NoScale = 2, // 0010
        NoScaleOrReflection = 6, // 0110
    }
}
