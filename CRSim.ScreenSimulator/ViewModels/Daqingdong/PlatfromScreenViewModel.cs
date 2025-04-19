using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.Daqingdong
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> Screen { get; set; } = [];
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            StationType = StationType.Departure;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            ItemsPerPage = 3;
            await WaitForDataLoadAsync();
            RefreshDisplay(null,null);
        }
        private void RefreshDisplay(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Screen.Clear();
                for (int i = 0; i < ItemsPerPage; i++)
                {
                    Screen.Add(TrainInfo.Count > i ? TrainInfo[i] : new());
                }
            });
        }
    }
}
