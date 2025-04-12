using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CRSim.ScreenSimulator.Converters
{
    public class TrainInfoToTicketCheckTimeConverter : IValueConverter
    {
        private ITimeService _timeService;
        private Settings _settings;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrainInfo trainInfo && trainInfo.DepartureTime!=null)
            {
                var serviceProvider = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
                _timeService = serviceProvider.GetRequiredService<ITimeService>();
                _settings = serviceProvider.GetRequiredService<ISettingsService>().GetSettings();
                if (trainInfo.ArrivalTime == null)
                {
                    return trainInfo.DepartureTime.Value.Subtract(_settings.DepartureCheckInAdvanceDuration).ToString("HH:mm");//始发车
                }
                return trainInfo.DepartureTime.Value.Subtract(_settings.PassingCheckInAdvanceDuration).ToString("HH:mm");//过路车
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
