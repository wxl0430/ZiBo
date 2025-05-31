using Microsoft.International.Converters.PinYinConverter;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CRSim.ScreenSimulator.Converters
{
    public class ChineseToPinyinConverter : IValueConverter
    {
        /// <summary>
        /// 将中文字符串转换为拼音（带声调或不带声调）
        /// </summary>
        /// <param name="value">要转换的中文字符串</param>
        /// <returns>拼音字符串</returns>
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
            return char.ToUpper(result[0], culture) + result[1..];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
