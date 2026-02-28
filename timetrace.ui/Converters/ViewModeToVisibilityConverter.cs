using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace timetrace.ui.Converters;

public class ViewModeToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string? current = value?.ToString();
        string? target = parameter?.ToString();

        bool match = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);

        return match ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
