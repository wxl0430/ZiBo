using System.Globalization;
using System.Windows.Data;

namespace CRSim.ScreenSimulator.Converters
{
    public class TextSpacingConverter : IValueConverter
    {
        public int TextLength { get; set; } = 2;
        public string Spacing { get; set; } = "   ";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && s.Length == TextLength)
            {
                return string.Join(Spacing, s.ToCharArray());
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
