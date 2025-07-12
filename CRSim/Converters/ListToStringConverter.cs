using Microsoft.UI.Xaml.Data;

namespace CRSim.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        public string Separator { get; set; } = "";  // 默认分隔符是双引号间隔
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is List<string> ticketChecks)
            {
                // 使用自定义的 Separator 将字符串连接
                return string.Join(Separator, ticketChecks);
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // 通常不需要实现 ConvertBack
            return null;
        }
    }
}
