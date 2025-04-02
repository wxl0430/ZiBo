using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.ChengduDong
{
    public class PrimaryTicketCheckScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> Screen { get; set; } = [];
        public PrimaryTicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "持户口簿、护照、临时身份证的旅客，请在非居民身份证通道排队。请主动礼让老、弱、病、残、孕的旅客。";
            StationType = StationType.Departure;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            ItemsPerPage = 2;
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
