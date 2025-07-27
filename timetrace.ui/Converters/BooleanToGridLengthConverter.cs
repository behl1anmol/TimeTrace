using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace timetrace.ui.Converters;

public class BooleanToGridLengthConverter : IValueConverter
{
    public double CollapsedWidth { get; set; } = 120;
    public double ExpandedWidth { get; set; } = 250;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isCollapsed)
        {
            return new GridLength(isCollapsed ? CollapsedWidth : ExpandedWidth);
        }
        return new GridLength(ExpandedWidth);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}