using System.Linq;
using System;
using System.Globalization;

namespace CRSim.ViewModels
{
    public partial class ScreenViewModel : ObservableObject
    {
        private readonly ITimeService _timeService;
        public readonly Settings _settings;
        private readonly TaskCompletionSource<bool> _dataLoaded = new();
        [ObservableProperty]
        private DateTime _currentTime = new();
        [ObservableProperty]
        private string _text = "";
        [ObservableProperty]
        private Station _thisStation;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<TrainInfo> TrainInfo { get; set; } = [];

        protected ScreenViewModel(ITimeService timeService, ISettingsService settingsService)
        {

            _timeService = timeService;
            _settings = settingsService.GetSettings();

            _timeService.OneSecondElapsed += OnTimeElapsed;
            _timeService.RefreshSecondsElapsed += RefreshData;
        }
        public int CurrentPageIndex = 0;
        public int ItemsPerPage;
        public int PageCount;

        public virtual void RefreshData(object? sender, EventArgs e)
        {
            if (CurrentPageIndex==0)
            {
                List<TrainInfo> itemsToRemove = [.. TrainInfo.Where(info => info.DepartureTime.Subtract(_settings.StopDisplayUntilDepartureDuration) < _timeService.GetDateTimeNow())];

                foreach (var item in itemsToRemove)
                {
                    TrainInfo.Remove(item);
                }
            }
        }

        private void OnTimeElapsed(object? sender, EventArgs e)
        {
            CurrentTime = _timeService.GetDateTimeNow();
        }

        public Task WaitForDataLoadAsync() => _dataLoaded.Task;

        public void LoadData(Station station,string ticketCheck,string platform)
        {
            ThisStation = station;
            var trains = station.StationStops;
            foreach (var trainNumber in trains)
            {

                if (trainNumber != null && trainNumber.DepartureTime != null && (ticketCheck==string.Empty || trainNumber.TicketChecks.Contains(ticketCheck)) && (platform == string.Empty || trainNumber.Platform==platform))
                {
                    TrainInfo.Add(new TrainInfo
                    {
                        TrainNumber = trainNumber.Number,
                        Terminal = trainNumber.Terminal,
                        Origin = trainNumber.Origin,
                        ArrivalTime = trainNumber.ArrivalTime == null ? null : trainNumber.DepartureTime.Value < _timeService.GetDateTimeNow() ? trainNumber.ArrivalTime.Value.AddDays(1) : trainNumber.ArrivalTime.Value,
                        DepartureTime = trainNumber.DepartureTime.Value < _timeService.GetDateTimeNow() ? trainNumber.DepartureTime.Value.AddDays(1) : trainNumber.DepartureTime.Value,
                        TicketChecks = trainNumber.TicketChecks,
                        WaitingArea = station.WaitingAreas.Where(x => x.TicketChecks.Intersect(trainNumber.TicketChecks).ToList().Count!=0).FirstOrDefault().Name,
                        Platform = trainNumber.Platform,
                        Length  =trainNumber.Length,
                        Landmark = trainNumber.Landmark,
                        State = TimeSpan.Zero
                    });
                }
            }
            TrainInfo = [.. TrainInfo.OrderBy(x => x.DepartureTime)];
            RefreshData(null, null);
            _dataLoaded.SetResult(true);
        }

    }
}
