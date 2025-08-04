using Microsoft.UI.Xaml.Data;
using System.Globalization;

namespace CRSim.Converters
{
    public partial class TimeSpanToStringConverter : IValueConverter
    {

        public string Format { get; set; } = @"hh\:mm";
        public string Culture { get; set; } = "zh-CN";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan dateTime && dateTime != new TimeSpan())
            {
                var CultureInfo = new CultureInfo(Culture);
                return (dateTime-dateTime.Days*TimeSpan.FromHours(24)).ToString(Format,CultureInfo);
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
