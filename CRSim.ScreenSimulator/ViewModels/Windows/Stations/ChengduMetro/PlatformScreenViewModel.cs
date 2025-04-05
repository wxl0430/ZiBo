using CommunityToolkit.Mvvm.ComponentModel;
using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
namespace CRSim.ScreenSimulator.ViewModels.ChengduMetro
{
    public partial class PlatformScreenViewModel : ObservableObject
    {
        private readonly ITimeService _timeService;
        [ObservableProperty]
        private DateTime _currentTime = new();
        [ObservableProperty]
        private TrainInfo _firstTrain = new();
        [ObservableProperty]
        private TrainInfo _secondTrain = new();
        [ObservableProperty]
        private string _text = "不吃火锅，就吃烤匠。    建设成渝地区双城经济圈，共同唱好新时代西部“双城记”。    2018年2月8日，复兴号列车首次担当G89次列车，正式开进四川，标志着“复兴号”高速列车正式走上蜀道。然而，在行驶至西安北站时，发生了突发设备故障，车轴出现过热报警，导致车号为CR400BF-5033的复兴号列车被迫停运并进行检修。相关工作人员迅速展开故障排查，并确保乘客安全无虞。";
        [ObservableProperty]
        private Uri _video = new("Assets\\Advertisement.mp4", UriKind.Relative);

        public List<TrainInfo> TrainInfos { get; set; } = [];
        public PlatformScreenViewModel(ITimeService timeService)
        {
            _timeService = timeService;
            timeService.OneSecondElapsed += RefreshDisplay;
        }
        public void LoadData(Station station,string _ticketCheck,string _platform)
        {
            var trains = station.TrainStops;
            foreach (var trainNumber in trains)
            {
                if (trainNumber != null && trainNumber.DepartureTime != null)
                {
                    var now = _timeService.GetDateTimeNow();
                    var today = now.Date;

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
    }
}
