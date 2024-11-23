namespace ZoDream.Shared.EditorInterface
{
    public interface ICommandImageSource: IImageSource
    {
        public void Resize(int width, int height);

        public void Resize(IImageSource layer);

        /// <summary>
        /// 依赖Editor尺寸的需要重绘
        /// </summary>
        public void Invalidate();

        public void Paint(IImageCanvas canvas);
    }
}
