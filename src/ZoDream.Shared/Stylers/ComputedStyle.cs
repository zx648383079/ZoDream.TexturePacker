using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Stylers
{
    public class ComputedStyle : IReadOnlyStyle
    {
        public ComputedStyle(IReadOnlyStyle source)
        {
            X = source.X;
            Y = source.Y;
            Rotate = source.Rotate;
            ScaleX = source.ScaleX;
            ScaleY = source.ScaleY;
            ShearX = source.ShearX;
            ShearY = source.ShearY;
        }

        public ComputedStyle(IReadOnlyStyle source, IReadOnlyStyle parent)
        {
            // TODO 算法有问题
            X = source.X + parent.X;
            Y = source.Y + parent.Y;
            Rotate = source.Rotate + parent.Rotate;
            ScaleX = source.ScaleX * parent.ScaleX;
            ScaleY = source.ScaleY * parent.ScaleY;
            ShearX = source.ShearX + parent.ShearX;
            ShearY = source.ShearY + parent.ShearY;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Rotate { get; private set; }

        public float ScaleX { get; private set; }
        public float ScaleY { get; private set; }
        /// <summary>
        /// 错切 相当 skewX
        /// </summary>
        public float ShearX { get; private set; }
        public float ShearY { get; private set; }
    }
}
