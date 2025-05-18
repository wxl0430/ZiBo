using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.Zibo
{
    public class TicketCheckScreenViewModel : ScreenViewModel
    {
        public TicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"    开车前{settingsService.GetSettings().StopCheckInAdvanceDuration.TotalMinutes}分钟停止检票";
            StationType = StationType.Departure;
            ItemsPerPage = 5;
            PageCount = 2;
        }
        public override void RefreshDisplay(object? sender, EventArgs? e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                int pageCount = (int)Math.Ceiling((double)TrainInfo.Count / (ItemsPerPage * PageCount.Value));
                LeftScreen.Clear();
                RightScreen.Clear();
                int startIndex = CurrentPageIndex * ItemsPerPage * PageCount.Value;
                var ItemsToShow = TrainInfo.Skip(startIndex).Take(ItemsPerPage * PageCount.Value).ToList();
                var leftItems = ItemsToShow.Where((item, index) => index % 2 == 0).ToList();
                var rightItems = ItemsToShow.Where((item, index) => index % 2 == 1).ToList();
                while (leftItems.Count < ItemsPerPage)
                {
                    leftItems.Add(new TrainInfo());
                }
                while (rightItems.Count < ItemsPerPage)
                {
                    rightItems.Add(new TrainInfo());
                }
                foreach (var item in leftItems)
                {
                    LeftScreen.Add(item);
                }
                foreach (var item in rightItems)
                {
                    RightScreen.Add(item);
                }
                CurrentPageIndex = CurrentPageIndex + 1 >=  Math.Min(_settings.MaxPages, pageCount) ? 0 : CurrentPageIndex + 1;
            });
        }
    }
}
