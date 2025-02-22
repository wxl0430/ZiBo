namespace CRSim.Converters
{
    public class TrainStateColorConverter : IMultiValueConverter
    {
        private ITimeService _timeService;
        private Settings _settings;
        public SolidColorBrush WaitingColor { get; set; } = new(Colors.White);
        object IMultiValueConverter.Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var serviceProvider = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
            _timeService = serviceProvider.GetRequiredService<ITimeService>();
            _settings = serviceProvider.GetRequiredService<ISettingsService>().GetSettings();
            if (values[1] is DateTime departureTime && departureTime !=new DateTime())
            {
                if (values[0] is DateTime)
                {
                    //过路站
                    if (_timeService.GetDateTimeNow() > departureTime.Subtract(_settings.PassingCheckInAdvanceDuration) && _timeService.GetDateTimeNow() < departureTime.Subtract(_settings.StopCheckInAdvanceDuration))
                    {
                        return new SolidColorBrush(Colors.LightGreen);
                    }
                }
                else
                {
                    //始发站
                    if (_timeService.GetDateTimeNow() > departureTime.Subtract(_settings.DepartureCheckInAdvanceDuration) && _timeService.GetDateTimeNow() < departureTime.Subtract(_settings.StopCheckInAdvanceDuration))
                    {
                        return new SolidColorBrush(Colors.LightGreen);
                    }
                }
                if (_timeService.GetDateTimeNow() > departureTime.Subtract(_settings.StopCheckInAdvanceDuration))
                {
                    return new SolidColorBrush(Colors.Red);
                }
                if (values[2] is TimeSpan state && state.TotalMinutes > 0)
                {
                    return new SolidColorBrush(Colors.Red);
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
