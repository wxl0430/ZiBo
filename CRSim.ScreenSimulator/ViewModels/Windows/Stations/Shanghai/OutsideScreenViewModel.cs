using CRSim.Core.Services;
using CRSim.Core.Models;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.Shanghai
{
    public class OutsideScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> Screen { get; private set; } = [];
        public OutsideScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 9;
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
                Screen.Clear();
                int startIndex = CurrentPageIndex * ItemsPerPage * PageCount;
                var items = TrainInfo.Skip(startIndex).Take(ItemsPerPage).ToList();
                while (items.Count < ItemsPerPage) 
                {
                    items.Add(new TrainInfo());
                }
                foreach (var item in items)
                {
                    Screen.Add(item);
                }
                CurrentPageIndex = CurrentPageIndex + 1 >= Math.Min(_settings.MaxPages, pageCount) ? 0 : CurrentPageIndex + 1;
            });
        }
    }
}
