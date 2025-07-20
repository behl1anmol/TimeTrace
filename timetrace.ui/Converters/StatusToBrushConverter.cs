using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace timetrace.ui.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            switch (status.ToLowerInvariant())
            {
                case "active":
                    return new SolidColorBrush(Colors.LimeGreen);
                case "minimized":
                    return new SolidColorBrush(Colors.Goldenrod);
                case "closed":
                    return new SolidColorBrush(Colors.Gray);
                default:
                    return new SolidColorBrush(Colors.LightGray);
            }
        }
        return new SolidColorBrush(Colors.LightGray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
