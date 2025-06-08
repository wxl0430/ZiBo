using Microsoft.International.Converters.PinYinConverter;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CRSim.ScreenSimulator.Converters
{
    public class ChineseToPinyinConverter : IValueConverter
    {
        public bool Upper { get; set; } = false;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            string chineseText = value.ToString();
            if (string.IsNullOrWhiteSpace(chineseText))
                return string.Empty;

            var sb = new StringBuilder();
            foreach (char c in chineseText)
            {
                if (ChineseChar.IsValidChar(c))
                {
                    var chineseChar = new ChineseChar(c);
                    var pinyins = chineseChar.Pinyins;
                    if (pinyins != null && pinyins.Count > 0)
                    {
                        var pinyin = pinyins[0][..^1];
                        sb.Append(pinyin);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            var result = sb.ToString().ToLower();
            return Upper ? result.ToUpper() : char.ToUpper(result[0], culture) + result[1..];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
