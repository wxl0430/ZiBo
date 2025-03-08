using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSim.Converters
{
    public class NumberToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int l)
            {
                if (l % 2 == 0)
                {
                    return (new List<string> { $"1-{l / 2}", $"{l / 2 + 1}-{l}" })[l.GetHashCode() % 2];
                }
                else
                {
                    return (new List<string> { $"1-{(l + 1) / 2}", $"{(l + 1) / 2}-{l}" })[l.GetHashCode() % 2];
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
