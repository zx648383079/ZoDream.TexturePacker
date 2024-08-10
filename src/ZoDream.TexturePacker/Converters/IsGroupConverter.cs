﻿using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.ObjectModel;
using ZoDream.TexturePacker.ViewModels;

namespace ZoDream.TexturePacker.Converters
{
    public class IsGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<LayerViewModel> items)
            {
                return items is not null && items.Count > 0;
            }
            if (value is int i)
            {
                return i > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}