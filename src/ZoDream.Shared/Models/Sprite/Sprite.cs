using System.Collections.Generic;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Models
{
    public class SpriteLayerSection: ISpriteSection
    {
        public bool UseCustomName { get; set; }
        /// <summary>
        /// 原点的位置
        /// </summary>
        public OriginPositionType Origin { get; set; } = OriginPositionType.LeftTop;
        /// <summary>
        /// 坐标轴的方向
        /// </summary>
        public CoordinateDirectionType Coordinate { get; set; } = CoordinateDirectionType.Normal;
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public float Width { get; set; }
        public float Height { get; set; }

        public IList<ISpriteLayer> Items { get; set; } = [];
        

        public float ComputeX(IImageSize size, IImageBound bound)
        {
            var x = bound.X;
            if (Coordinate.HasFlag(CoordinateDirectionType.Left))
            {
                x = (x + size.Width) * -1;
            }
            return Origin switch
            {
                OriginPositionType.RightTop or OriginPositionType.RightBottom => x + size.Width,
                OriginPositionType.Center => x + (size.Width / 2),
                _ => x,
            };
        }

        public float ComputeX(IImageBound bound)
        {
            return ComputeX(this, bound);
        }

        public float ComputeY(IImageSize size, IImageBound bound)
        {
            var y = bound.Y;
            if (Coordinate.HasFlag(CoordinateDirectionType.Up))
            {
                y = (y + size.Height) * -1;
            }
            return Origin switch
            {
                OriginPositionType.LeftBottom or OriginPositionType.RightBottom => y + size.Height,
                OriginPositionType.Center => y + (size.Height / 2),
                _ => y,
            };
        }

        public float ComputeY(IImageBound bound)
        {
            return ComputeY(this, bound);
        }
    }
}
