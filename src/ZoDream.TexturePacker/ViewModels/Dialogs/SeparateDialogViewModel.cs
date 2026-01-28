using Microsoft.UI.Xaml;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.ViewModel;

namespace ZoDream.TexturePacker.ViewModels
{
    public class SeparateDialogViewModel : BindableBase,
        IFormValidator
    {

        public string[] TypeItems { get; } = ["自动拆分", "数量拆分", "尺寸拆分", "单个拆分"];

        private int _selectedIndex;

        public int SelectedIndex {
            get => _selectedIndex;
            set {
                Set(ref _selectedIndex, value);
                OnPropertyChanged(nameof(OffsetVisible));
                OnPropertyChanged(nameof(WidthLabel));
                OnPropertyChanged(nameof(HeightLabel));
            }
        }

        private int _x;

        public int X {
            get => _x;
            set => Set(ref _x, value);
        }

        private int _y;

        public int Y {
            get => _y;
            set => Set(ref _y, value);
        }

        private int _width;

        public int Width {
            get => _width;
            set {
                Set(ref _width, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private int _height;

        public int Height {
            get => _height;
            set {
                Set(ref _height, value); 
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private bool _isMerge;

        public bool IsMerge {
            get => _isMerge;
            set => Set(ref _isMerge, value);
        }

        public Visibility OffsetVisible => SelectedIndex == 3 ? Visibility.Visible : Visibility.Collapsed;

        public string WidthLabel => SelectedIndex == 1 ? "水平数量" : "宽度(px)";
        public string HeightLabel => SelectedIndex == 1 ? "垂直数量" : "高度(px)";

        public bool IsValid => Width > 0 && Height > 0;

        public async Task<SKPath[]> SplitAsync(SKBitmap source)
        {
            switch (SelectedIndex)
            {
                case 3:
                    {
                        var path = new SKPath();
                        path.AddRect(SKRect.Create(X, Y, Width, Height));
                        return [path];
                    }
                case 2:
                    {
                        var countX = (int)Math.Floor((double)source.Width / Width);
                        var countY = (int)Math.Floor((double)source.Height / Height);
                        var res = new SKPath[countX * countY];
                        for (int i = 0; i < countX; i++)
                        {
                            for (int j = 0; j < countY; j++)
                            {
                                var path = new SKPath();
                                path.AddRect(SKRect.Create(i * Width, j * Height, Width, Height));
                                res[i * countY + j] = path;
                            }
                        }
                        return res;
                    }
                case 1:
                    {
                        var res = new SKPath[Width * Height];
                        var chunkX = source.Width / Width;
                        var chunkY = source.Height / Height;
                        for (int i = 0; i < Width; i++)
                        {
                            for (int j = 0; j < Height; j++)
                            {
                                var path = new SKPath();
                                path.AddRect(SKRect.Create(i * chunkX, j * chunkY, chunkX, chunkY));
                                res[i * Height + j] = path;
                            }
                        }
                        return res;
                    }
                default:
                    return await new ImageContourTrace(true).GetContourAsync(source);
            }
        }
    }
}
