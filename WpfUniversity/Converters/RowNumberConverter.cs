using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfUniversity.Converters;

public class RowNumberConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3 ||
            !(values[0] is DataGridRow row) ||
            !(values[1] is int currentPage) ||
            !(values[2] is int pageSize))
        {
            return "0";
        }

        int rowIndex = row.GetIndex();
        int overallRowNumber = (currentPage - 1) * pageSize + rowIndex + 1;
        return overallRowNumber.ToString();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
