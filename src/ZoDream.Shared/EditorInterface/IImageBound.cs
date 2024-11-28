namespace ZoDream.Shared.EditorInterface
{
    public interface IImageBound: IImagePoint, IImageSize
    {
    }

    public interface IImagePoint
    {
        public float X { get; set; }

        public float Y { get; set; }

    }

    public interface IImageSize
    {
        public float Width { get; set; }
        public float Height { get; set; }

    }
}
