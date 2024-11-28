namespace ZoDream.Shared.EditorInterface
{
    public interface ICommandImageSource: IImageSource
    {
        public void Resize(float width, float height);
        /// <summary>
        /// 设置面向的图层
        /// </summary>
        /// <param name="layer"></param>
        public void With(IImageLayer layer);

        /// <summary>
        /// 依赖Editor尺寸的需要重绘
        /// </summary>
        public void Invalidate();

        public void Paint(IImageCanvas canvas);
    }
}
