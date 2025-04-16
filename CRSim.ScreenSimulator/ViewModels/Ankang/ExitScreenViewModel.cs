using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using CRSim.Core.Models;
using System.Windows;

namespace CRSim.ScreenSimulator.ViewModels.Ankang
{
    public class ExitScreenViewModel : ScreenViewModel
    {
        private ITimeService _timeService;
        public ObservableCollection<TrainInfo> LeftScreen { get; private set; } = [];
        public ObservableCollection<TrainInfo> RightScreen { get; private set; } = [];
        public ExitScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            _timeService = timeService;
            Text = $"西安局集团公司推出西铁行APP站车服务产品，欢迎体验使用。";
            ItemsPerPage = 7;
            PageCount = 1;
            StationType = StationType.Arrival;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            await WaitForDataLoadAsync();
            RefreshDisplay(null,null);
        }
        private void RefreshDisplay(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                int pageCount = (int)Math.Ceiling((double)TrainInfo.Count / (ItemsPerPage * PageCount));
                LeftScreen.Clear();
                RightScreen.Clear();
                int startIndex = CurrentPageIndex * ItemsPerPage * PageCount;
                var leftItems = TrainInfo.Skip(startIndex).Take(ItemsPerPage).ToList();
                while (leftItems.Count < ItemsPerPage) // Fill up with new object() if not enough items
                {
                    leftItems.Add(new TrainInfo());
                }
                foreach (var item in leftItems)
                {
                    LeftScreen.Add(item);
                }

                // Get the slice for the right screen (next 12)
                var rightItems = TrainInfo.Skip(startIndex + ItemsPerPage).Take(ItemsPerPage).ToList();
                while (rightItems.Count < ItemsPerPage) // Fill up with new object() if not enough items
                {
                    rightItems.Add(new TrainInfo());
                }
                foreach (var item in rightItems)
                {
                    RightScreen.Add(item);
                }

                CurrentPageIndex = CurrentPageIndex + 1 >= Math.Min(_settings.MaxPages, pageCount) ? 0 : CurrentPageIndex + 1;
            });
        }
        public override void RefreshData(object? sender, EventArgs e)
        {
            if (CurrentPageIndex == 0)
            {
                List<TrainInfo> itemsToRemove = [];
                foreach (TrainInfo trainInfo in TrainInfo)
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
                        if (trainInfo.DepartureTime.Value < _timeService.GetDateTimeNow())
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

    }
}
