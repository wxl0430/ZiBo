using CommunityToolkit.Mvvm.ComponentModel;
using CRSim.Core.Services;
using CRSim.Core.Models;
using CRSim.ScreenSimulator.Models;
using System.ComponentModel;

namespace CRSim.ScreenSimulator.ViewModels
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
        public int _location;
        [ObservableProperty]
        private Station _thisStation;
        [ObservableProperty]
        private string _thisPlatform;
        [ObservableProperty]
        private string _thisTicketCheck;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<TrainInfo> TrainInfo { get; set; } = [];

        public StationType StationType = StationType.Both;

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
                List<TrainInfo> itemsToRemove = [];
                foreach(TrainInfo trainInfo in TrainInfo)
                {
                    if (trainInfo.DepartureTime == null)
                    {
                        if (trainInfo.ArrivalTime.Value.Add(_settings.StopDisplayFromArrivalDuration) < _timeService.GetDateTimeNow())
                        {
                            itemsToRemove.Add(trainInfo);
                        }
                    }
                    else
                    {
                        if (trainInfo.DepartureTime.Value.Subtract(_settings.StopDisplayUntilDepartureDuration) < _timeService.GetDateTimeNow())
                        {
                            itemsToRemove.Add(trainInfo);
                        }
                    }
                }
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

        public void LoadData(Station station, string ticketCheck, string platform)
        {
            ThisStation = station;
            ThisPlatform = platform;
            if (ticketCheck != string.Empty) ThisTicketCheck = ticketCheck.Split(" - ")[1];//重复检票口名称的临时解决方案
            var trains = station.TrainStops;
            foreach (var trainNumber in trains)
            {

                if (trainNumber != null &&
                    (StationType == StationType.Both ||
                    trainNumber.StationType == StationType.Both ||
                    StationType == trainNumber.StationType) &&
                    (ticketCheck == string.Empty || (trainNumber.WaitingArea==ticketCheck.Split(" - ")[0] && trainNumber.TicketChecks.Contains(ticketCheck.Split(" - ")[1]))) &&//重复检票口名称的临时解决方案
                    (platform == string.Empty || trainNumber.Platform == platform))
                {
                    var now = _timeService.GetDateTimeNow();
                    var today = now.Date;

                    DateTime? AdjustTime(TimeSpan? time) =>
                        time.HasValue ? (today.Add(time.Value) > now ? today.Add(time.Value) : today.Add(time.Value).AddDays(1)) : null;

                    TrainInfo.Add(new TrainInfo
                    {
                        TrainNumber = trainNumber.Number,
                        Terminal = trainNumber.Terminal,
                        Origin = trainNumber.Origin,
                        ArrivalTime = AdjustTime(trainNumber.ArrivalTime),
                        DepartureTime = AdjustTime(trainNumber.DepartureTime),
                        TicketChecks = trainNumber.TicketChecks,
                        WaitingArea = trainNumber.StationType == StationType.Arrival
                            ? string.Empty
                            : station.WaitingAreas.FirstOrDefault(x => x.TicketChecks.Intersect(trainNumber.TicketChecks).Any())?.Name ?? string.Empty,
                        Platform = trainNumber.Platform,
                        Length = trainNumber.Length,
                        Landmark = trainNumber.Landmark,
                        State = TimeSpan.Zero
                    });

                }
            }
            TrainInfo = [.. TrainInfo.OrderBy(x => x.DepartureTime??x.ArrivalTime)];
            RefreshData(null, null);
            _dataLoaded.SetResult(true);
        }

    }
}
