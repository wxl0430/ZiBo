using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.BeijingNan
{
    public class PrimaryScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> LeftScreen { get; private set; } = [];
        public PrimaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 10;
            PageCount = 1;
            StationType = StationType.Departure;
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
                CurrentPageIndex = CurrentPageIndex + 1 >= Math.Min(_settings.MaxPages, pageCount) ? 0 : CurrentPageIndex + 1;
            });
        }
    }
}
