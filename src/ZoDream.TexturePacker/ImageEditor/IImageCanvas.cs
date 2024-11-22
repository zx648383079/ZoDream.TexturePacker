using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    public interface IImageCanvas : IImagePoint
    {
        public IImageCanvas Transform(int x, int y);

        public void DrawBitmap(SKBitmap? source, int x, int y);
        public void DrawSurface(SKSurface? surface, int x, int y);
        public void DrawPicture(SKPicture? picture, int x, int y);
        public void DrawText(string text, int x, int y, SKTextAlign textAlign, SKFont font, SKPaint paint);
    }
}
