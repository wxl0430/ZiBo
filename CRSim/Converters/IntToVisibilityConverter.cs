namespace CRSim.Converters;

/// <summary>
/// Converts a null value to Visibility.Collapsed
/// </summary>
internal sealed class IntToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is int intValue)
        {
            return intValue == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
