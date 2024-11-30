namespace ZoDream.Shared.Interfaces
{
    public interface IReadOnlyStyle
    {
        public float X { get; }
        public float Y { get; }
        public float Rotate { get; }

        public float ScaleX { get; }
        public float ScaleY { get; }
        /// <summary>
        /// 错切 相当 skewX
        /// </summary>
        public float ShearX { get; }
        public float ShearY { get; }
    }
}
