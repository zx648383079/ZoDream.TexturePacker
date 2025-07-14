using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace ZoDream.TexturePacker.Converters
{
    public static class ConverterHelper
    {
        public static string Format(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return "-";
            }
            return date.ToString("yyyy-MM-dd HH:mm");
        }


        public static string FormatSize(long size)
        {
            return SizeConverter.FormatSize(size);
        }


        public static string FormatHour(int value)
        {
            if (value <= 0)
            {
                return "00:00";
            }
            var m = value / 60;
            if (m >= 60)
            {
                return (m / 60).ToString("00") + ":"
                    + (m % 60).ToString("00") + ":" + (value % 60).ToString("00");
            }
            return m.ToString("00") + ":" + (value % 60).ToString("00");
        }

        public static Visibility VisibleIf(bool val)
        {
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Visibility VisibleIf(string val)
        {
            return VisibleIf(!string.IsNullOrWhiteSpace(val));
        }

        public static Visibility VisibleIf(int val)
        {
            return VisibleIf(val > 0);
        }
        public static Visibility CollapsedIf(bool val)
        {
            return VisibleIf(!val);
        }

    }
}
