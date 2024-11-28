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

        public float ActualLeft { get; }
        public float ActualTop { get; }

        public float ActualWidth { get; }
        public float ActualHeight { get; }

        public float ActualOuterWidth { get; }
        public float ActualOuterHeight { get; }
        /// <summary>
        /// 重新计算
        /// </summary>
        public void Compute();

    }
}
