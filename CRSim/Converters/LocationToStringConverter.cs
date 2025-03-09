namespace CRSim.Converters
{
    public class LocationToStringConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is int Length&& values[1] is int Location)
            {
                if (Location > Length)
                {
                    return $"当前无车，1-{Length}车向后。";
                }
                if (Location == Length)
                {
                    return $"当前{Length}车，1-{Length-1}车向后。";
                }
                if (Location == 1)
                {
                    return $"当前1车，2-{Length}车向前。";
                }
                return $"当前{Location}车，1-{Location - 1}车向前，{Location + 1}-{Length}车向后。";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // 通常不需要实现 ConvertBack
            return null;
        }
    }
}
