namespace ZoDream.Shared.EditorInterface
{
    public interface IImageStyle: IImageBound
    {

        /// <summary>
        /// 旋转角度0 - 360
        /// </summary>
        public float Rotate { get; set; }

        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
    }

    public interface IImageComputedStyle : IImageStyle
    {
        public int ZIndex { get; set; }

        public int ActualLeft { get; }
        public int ActualTop { get; }

        public int ActualWidth { get; }
        public int ActualHeight { get; }

        public int ActualOuterWidth { get; }
        public int ActualOuterHeight { get; }
        /// <summary>
        /// 重新计算
        /// </summary>
        public void Compute();

    }
}
