using CRSim.Core.Models;
using CRSim.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace CRSim.ScreenSimulator.Converters
{
    public class TrainStateColorConverter : IMultiValueConverter
    {
        private ITimeService _timeService;
        private Settings _settings;
        public string DisplayMode { get; set; } = "Normal";

        /*
        Normal: 标准显示。
        Alternating_Row_Colors: 候车状态隔行异色显示（第4个参数控制行号）。
        */

        public List<SolidColorBrush> WaitingColorList { get; set; } = new List<SolidColorBrush> { new(Colors.White), new(Colors.Yellow) };
        public SolidColorBrush WaitingColor { get; set; } = new(Colors.White);
        public SolidColorBrush CheckingTicketsColor { get; set; } = new(Colors.LightGreen);
        public SolidColorBrush StopCheckingTicketsColor { get; set; } = new(Colors.Red);
        object IMultiValueConverter.Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var serviceProvider = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
            _timeService = serviceProvider.GetRequiredService<ITimeService>();
            _settings = serviceProvider.GetRequiredService<ISettingsService>().GetSettings();
            if (values[0] != null && values[1] == null)
            {
                return WaitingColor;
            }
            if (values[1] is DateTime departureTime && departureTime !=new DateTime())
            {
                if (values[0] is DateTime)
                {
                    //过路站
                    if (_timeService.GetDateTimeNow() > departureTime.Subtract(_settings.PassingCheckInAdvanceDuration) && _timeService.GetDateTimeNow() < departureTime.Subtract(_settings.StopCheckInAdvanceDuration))
                    {
                        return CheckingTicketsColor;
                    }
                }
                else
                {
                    //始发站
                    if (_timeService.GetDateTimeNow() > departureTime.Subtract(_settings.DepartureCheckInAdvanceDuration) && _timeService.GetDateTimeNow() < departureTime.Subtract(_settings.StopCheckInAdvanceDuration))
                    {
                        return CheckingTicketsColor;
                    }
                }
                if (_timeService.GetDateTimeNow() > departureTime.Subtract(_settings.StopCheckInAdvanceDuration))
                {
                    return StopCheckingTicketsColor;
                }
                if (values[2] is TimeSpan state && state.TotalMinutes > 0)
                {
                    return StopCheckingTicketsColor;
                }
                if (DisplayMode == "Alternating_Row_Colors" && values.Length > 3 && values[3] is int rowNumber){
                    return WaitingColorList[rowNumber % WaitingColorList.Count];
                }
                return WaitingColor;
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // 通常不需要实现 ConvertBack
            return null;
        }
    }
}
