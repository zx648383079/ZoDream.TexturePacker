﻿using SkiaSharp;

namespace ZoDream.TexturePacker.ImageEditor
{
    public class TextImageLayer(string text): IImageLayer
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Text { get; set; } = text;

        public SKTypeface? FontFamily { get; set; }

        public int FontSize { get; set; } = 16;

        public SKColor Color { get; set; } = SKColors.Black;

        public SKTextAlign TextAlign { get; set; } = SKTextAlign.Left;

        public void Paint(SKCanvas canvas, SKImageInfo info)
        {
            //FontFamily = SKTypeface.FromFile(Path.Combine(AppContext.BaseDirectory, "DroidSansFallback.ttf"));
            //FontFamily = SKTypeface.FromFamilyName("微软雅黑");
            using var paint = new SKPaint();
            paint.TextSize = FontSize;
            paint.IsAntialias = true;
            paint.Color = Color;
            paint.IsStroke = false;
            paint.TextAlign = TextAlign;
            // paint.GetGlyphWidths
            canvas.DrawText(Text, X, Y, paint);
        }

        public void Dispose()
        {

        }
    }
}
