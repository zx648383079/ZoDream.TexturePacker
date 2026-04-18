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
        

        public float ComputeX(IImageSize size, float x)
        {
            var scale = Coordinate.HasFlag(CoordinateDirectionType.Left) ? -1 : 1;
            return Origin switch
            {
                OriginPositionType.RightTop or OriginPositionType.RightBottom => (x * scale) + size.Width,
                OriginPositionType.Center => (x * scale) + (size.Width / 2),
                _ => x,
            };
        }

        public float ComputeX(float x)
        {
            return ComputeX(this, x);
        }

        public float ComputeY(IImageSize size, float y)
        {
            var scale = Coordinate.HasFlag(CoordinateDirectionType.Up) ? -1 : 1;
            return Origin switch
            {
                OriginPositionType.LeftBottom or OriginPositionType.RightBottom => (y * scale) + size.Height,
                OriginPositionType.Center => (y * scale) + (size.Height / 2),
                _ => y,
            };
        }

        public float ComputeY(float y)
        {
            return ComputeY(this, y);
        }
    }
}
