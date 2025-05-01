using CommunityToolkit.Mvvm.ComponentModel;
using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Runtime;
namespace CRSim.ScreenSimulator.ViewModels.XuzhouMetro
{
    public partial class PlatformScreenViewModel : ObservableObject
    {
        private readonly ITimeService _timeService;
        public readonly Settings _settings;
        [ObservableProperty]
        private DateTime _currentTime = new();
        [ObservableProperty]
        private TrainInfo _firstTrain = new();
        [ObservableProperty]
        private TrainInfo _secondTrain = new();
        [ObservableProperty]
        private string _text = "徐州，一座镌刻华夏文明印记的英雄之城。这里是大汉王朝的摇篮，楚风汉韵流淌在云龙山水间，龟山汉墓诉说着千年的传奇，汉画像石馆定格了古人的智慧光影。九里山前古战场，戏马台边秋风烈，历史的豪情从未褪色。作为五省通衢的枢纽，徐州以开放的胸怀连接南北，高铁飞驰、运河如练，现代脉动与古朴底蕴在此交融。登云龙山俯瞰一城青山半城湖，泛舟云龙湖感受水墨诗意；户部山的街巷藏着老徐州的烟火气，回龙窝的灯火点亮了夜色温柔。从地锅鸡的鲜香到饣它汤的醇厚，从酥脆的烙馍到清甜的蜜三刀，舌尖上的彭城满是热情滋味。这里是淮海之芯，用智造之光照亮未来，用楚汉气魄续写新章——徐州，等你共赴一场跨越时空的约定！";
        [ObservableProperty]
        private Uri _video = new("Assets\\Advertisement-2.mp4", UriKind.Relative);

        public List<TrainInfo> TrainInfos { get; set; } = [];
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
        {
            _timeService = timeService;
            _settings = settingsService.GetSettings();
            timeService.OneSecondElapsed += RefreshDisplay;
        }
        public void LoadData(Station station,string _ticketCheck,string platform)
        {
            var trains = station.TrainStops;
            foreach (var trainNumber in trains)
            {
                if (trainNumber != null && trainNumber.DepartureTime != null && trainNumber.Platform==platform)
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
