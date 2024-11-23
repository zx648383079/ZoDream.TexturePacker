using SkiaSharp;

namespace ZoDream.Shared.EditorInterface
{
    public interface IImageCanvas : IImagePoint
    {
        public IImageCanvas Transform(int x, int y);
        public IImageStyle Compute(IImageLayer layer);
        public void DrawBitmap(SKBitmap? source, IImageStyle style);
        public void DrawSurface(SKSurface? surface, IImageStyle style);
        public void DrawSurface(SKSurface? surface, int x, int y);
        public void DrawPicture(SKPicture? picture, IImageStyle style);
        public void DrawText(string text, IImageStyle style, SKTextAlign textAlign, SKFont font, SKPaint paint);
        
    }
}
