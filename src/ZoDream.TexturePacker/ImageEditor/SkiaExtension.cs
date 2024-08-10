using SkiaSharp;
using System;

namespace ZoDream.TexturePacker.ImageEditor
{
    public static class SkiaExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="deg">360</param>
        public static void Rotate(this SKCanvas canvas, float deg)
        {
            canvas.RotateDegrees(deg);
        }

        public static void Flip(this SKCanvas canvas, bool isHorizontal = true)
        {
            if (isHorizontal)
            {
                canvas.Scale(1, -1);
            }
            else
            {
                canvas.Scale(-1, 1);
            }
        }

        /// <summary>
        /// 计算旋转后的外边框高度
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static (int, int) ComputedRotate(int width, int height, float angle)
        {
            var radians = Math.PI * angle / 180;
            var sine = Math.Abs(Math.Sin(radians));
            var cosine = Math.Abs(Math.Cos(radians));
            var originalWidth = width;
            var originalHeight = height;
            var rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            var rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);
            return (rotatedWidth, rotatedHeight);
        }

        public static SKBitmap Rotate(this SKBitmap bitmap, float angle)
        {
            var(rotatedWidth, rotatedHeight) = ComputedRotate(bitmap.Width, bitmap.Height, angle);
            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);
            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees(angle);
                surface.Translate(-bitmap.Width / 2, - bitmap.Height / 2);
                surface.DrawBitmap(bitmap, new SKPoint());
            }
            return rotatedBitmap;
        }

        public static SKBitmap PaintRotate(this IImageLayer layer, float angle)
        {
            var (rotatedWidth, rotatedHeight) = ComputedRotate(layer.Width, layer.Height, angle);
            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);
            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees(angle);
                surface.Translate(-layer.Width / 2, -layer.Height / 2);
                layer.Paint(surface);
            }
            return rotatedBitmap;
        }
    }
}
