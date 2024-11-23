namespace ZoDream.Shared.EditorInterface
{
    public interface IImageCommander
    {
        public IImageLayerTree Source { get; }
        public IImageEditor Instance { set; }

        public IImageStyler Styler { get; }
        /// <summary>
        /// 一个默认的
        /// </summary>
        public IImageStyler DefaultStyler { get; }
        /// <summary>
        /// 一个实时的
        /// </summary>
        public IImageStyler RealStyler { get; }

        public IImageLayer Create(IImageSource source);
    }
}
