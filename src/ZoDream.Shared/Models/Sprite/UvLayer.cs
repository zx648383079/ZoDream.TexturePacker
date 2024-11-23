using System.Collections.Generic;
using System.Numerics;

namespace ZoDream.Shared.Models
{
    public class SpriteUvLayer: SpriteLayer
    {
        public IList<Vector2> VertexItems { get; set; } = [];
    }

}
