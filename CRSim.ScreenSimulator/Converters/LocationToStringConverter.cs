using System.Globalization;
using System.Windows.Data;

namespace CRSim.ScreenSimulator.Converters
{
    public class LocationToStringConverter : IMultiValueConverter
    {
        public string DisplayMode { get; set; } = "normal";
        object IMultiValueConverter.Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is int Length && values[1] is int Location)
            {
                if (DisplayMode == "less")
                {
                    if (Location > Length)
                    {
                        return $"当前无车厢，1~{Length}车厢向前。";
                    }
                    if (Location == Length)
                    {
                        return $"当前{Length}车厢，1车厢向前。";
                    }
                    if (Location == 1)
                    {
                        return $"当前1车厢，{Length}车厢向后。";
                    }
                    return $"1车厢向前，{Length}车厢向后。";
                }
                if (DisplayMode == "normal")
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
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // 通常不需要实现 ConvertBack
            return null;
        }
    }
}
