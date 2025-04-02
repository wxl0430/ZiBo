using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CRSim.ScreenSimulator.Converters
{
    public class MetroDateTimeToStringConverter : IValueConverter
    {
        private ITimeService _timeService;
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var serviceProvider = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
            _timeService = serviceProvider.GetRequiredService<ITimeService>();
            if (value is TrainInfo trainInfo && parameter is string para)
            {
                if(trainInfo.TrainNumber == null)
                {
                    return "请耐心等待";
                }
                var timeDifference = (trainInfo.ArrivalTime ?? DateTime.MinValue) - _timeService.GetDateTimeNow();
                bool isArrivalTimeNull = trainInfo.ArrivalTime == null;
                var checkInAdvanceDuration = TimeSpan.FromMinutes(1);
                if (timeDifference < checkInAdvanceDuration)
                {
                    if (para == "TextBlock") 
                    {
                        if (!isArrivalTimeNull) 
                        {
                            if(trainInfo.ArrivalTime - _timeService.GetDateTimeNow() > TimeSpan.Zero)
                            {
                                return "即将进站";
                            }
                        }
                        return "列车进站";
                    }
                }
                else
                {
                    if (para == "StackPanel")
                        return Math.Round((trainInfo.ArrivalTime - _timeService.GetDateTimeNow()).Value.TotalMinutes).ToString();
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
