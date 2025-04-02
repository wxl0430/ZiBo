using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using CRSim.Core.Models;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.Fuzhou
{
    public class TicketCheckScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> Screen { get; set; } = [];
        public TicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"开车前{settingsService.GetSettings().StopCheckInAdvanceDuration.TotalMinutes}分钟停止检票";
            StationType = StationType.Departure;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            ItemsPerPage = 4;
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
