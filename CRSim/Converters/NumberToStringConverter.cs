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
            if(value is string number)
            {
                if(number.StartsWith('G')|| number.StartsWith('D')|| number.StartsWith('C'))
                {
                    return Math.Abs(number.GetHashCode()) % 3 == 0 ? "1-8" : "9-16";
                }
                else
                {
                    return Math.Abs(number.GetHashCode()) % 2 == 0 ? "1-9" : "10-18";
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
