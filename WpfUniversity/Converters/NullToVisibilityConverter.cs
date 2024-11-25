﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfUniversity.Converters;

public class NullToVisibilityConverter : IValueConverter
{
    public bool Inverse { get; set; } = false;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNull = value == null;
        if (Inverse)
            isNull = !isNull;
        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
