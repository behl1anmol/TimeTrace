using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace timetrace.ui.Converters;

public class IntToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool invert = parameter is string s && s.Equals("True", StringComparison.OrdinalIgnoreCase);
        bool isPositive = value is int intValue && intValue > 0;

        return (isPositive ^ invert) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
