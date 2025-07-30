using CommunityToolkit.Mvvm.ComponentModel;
using CRSim.Core.Abstractions;
using CRSim.Core.Models;
using CRSim.ScreenSimulator.Models;

namespace CRSim.ScreenSimulator.ViewModels
{
    public partial class MetroPlatformScreenViewModel : ObservableObject
    {
        public readonly ITimeService _timeService;
        public readonly Settings _settings;
        [ObservableProperty]
        private DateTime _currentTime = new();
        [ObservableProperty]
        private TrainInfo _firstTrain = new();
        [ObservableProperty]
        private TrainInfo _secondTrain = new();
        [ObservableProperty]
        private string _text;
        [ObservableProperty]
        private Uri _video;
        public System.Windows.Threading.Dispatcher UIDispatcher { get; set; }
        public List<TrainInfo> TrainInfos { get; set; } = [];
        public MetroPlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
        {
            _timeService = timeService;
            _settings = settingsService.GetSettings();
            timeService.OneSecondElapsed += RefreshDisplay;
        }
        private void RefreshDisplay(object? sender, EventArgs e)
        {
            CurrentTime = _timeService.GetDateTimeNow();
            List<TrainInfo> itemsToRemove = [.. TrainInfos.Where(info => info.DepartureTime < _timeService.GetDateTimeNow())];
            foreach (var item in itemsToRemove)
            {
                TrainInfos.Remove(item);
            }
            FirstTrain = TrainInfos.Count > 0 ? TrainInfos[0] : new();
            SecondTrain = TrainInfos.Count > 1 ? TrainInfos[1] : new();

            OnPropertyChanged(nameof(FirstTrain));
            OnPropertyChanged(nameof(SecondTrain));
        }
        public void LoadData(Station station, string _ticketCheck, string platform)
        {
            var trains = station.TrainStops;
            foreach (var trainNumber in trains)
            {
                if (trainNumber != null && trainNumber.DepartureTime != null && trainNumber.Platform == platform)
                {
                    var now = _timeService.GetDateTimeNow();
                    var today = now.Date;
                    if (_settings.LoadTodayOnly && today.Add((trainNumber.DepartureTime ?? trainNumber.ArrivalTime)!.Value) < now)
                    {
                        continue;
                    }
                    DateTime? AdjustTime(TimeSpan? time) =>
                        time.HasValue ? (today.Add(time.Value) > now ? today.Add(time.Value) : today.Add(time.Value).AddDays(1)) : null;

                    TrainInfos.Add(new TrainInfo
                    {
                        TrainNumber = trainNumber.Number,
                        Terminal = trainNumber.Terminal,
                        ArrivalTime = AdjustTime(trainNumber.ArrivalTime),
                        DepartureTime = AdjustTime(trainNumber.DepartureTime),
                        State = TimeSpan.Zero
                    });
                }
            }
            TrainInfos = [.. TrainInfos.OrderBy(x => x.ArrivalTime ?? x.DepartureTime)];
        }

    }
}
