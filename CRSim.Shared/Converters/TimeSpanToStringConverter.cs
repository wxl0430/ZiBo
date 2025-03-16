using System.Globalization;
using System.Windows.Data;

namespace CRSim.Shared.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {

        public string Format { get; set; } = @"hh\:mm";
        public string Culture { get; set; } = "zh-CN";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan dateTime && dateTime != new TimeSpan())
            {
                var CultureInfo = new CultureInfo(Culture);
                return dateTime.ToString(Format,CultureInfo);
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
