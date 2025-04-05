using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.Zibo
{
    public class TicketCheckScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> LeftScreen { get; private set; } = [];
        public ObservableCollection<TrainInfo> RightScreen { get; private set; } = [];
        public TicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"    开车前{settingsService.GetSettings().StopCheckInAdvanceDuration.TotalMinutes}分钟停止检票";
            StationType = StationType.Departure;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            ItemsPerPage = 5;
            PageCount = 2;
            await WaitForDataLoadAsync();
            RefreshDisplay(null,null);
        }
        private void RefreshDisplay(object? sender, EventArgs? e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                int pageCount = (int)Math.Ceiling((double)TrainInfo.Count / (ItemsPerPage * PageCount));
                LeftScreen.Clear();
                RightScreen.Clear();
                int startIndex = CurrentPageIndex * ItemsPerPage * PageCount;
                var leftItems = TrainInfo.Skip(startIndex).Take(ItemsPerPage).ToList();
                var rightItems = TrainInfo.Skip(startIndex+1).Take(ItemsPerPage).ToList();
                while (leftItems.Count+rightItems.Count < ItemsPerPage*2) 
                {
                    leftItems.Add(new TrainInfo());
                    rightItems.Add(new TrainInfo());
                }
                foreach (var item in leftItems)
                {
                    LeftScreen.Add(item);
                }
                // while (rightItems.Count < ItemsPerPage) 
                // {
                //     rightItems.Add(new TrainInfo());
                // }
                foreach (var item in rightItems)
                {
                    RightScreen.Add(item);
                }
                CurrentPageIndex = CurrentPageIndex + 1 >=  Math.Min(_settings.MaxPages, pageCount) ? 0 : CurrentPageIndex + 1;
            });
        }
    }
}
