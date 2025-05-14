using CommunityToolkit.Mvvm.ComponentModel;
using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.ChengduDong
{
    public partial class PillarTicketCheckScreenViewModel : ScreenViewModel
    {
        [ObservableProperty]
        private TrainInfo _firstTrain = new();
        [ObservableProperty]
        private TrainInfo _secondTrain = new();
        [ObservableProperty]
        private TrainInfo _checkingTrain = new();
        public PillarTicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            StationType = StationType.Departure;
            timeService.OneSecondElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            await WaitForDataLoadAsync();
            RefreshDisplay(null, null);
        }
        private void RefreshDisplay(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (TrainInfo.Count == 0)
                {
                    return;
                }
                var now = _timeService.GetDateTimeNow();
                var departureTime = TrainInfo[0].DepartureTime!.Value;

                TimeSpan checkInStartOffset = TrainInfo[0].ArrivalTime is DateTime
                    ? _settings.PassingCheckInAdvanceDuration   // 过路站
                    : _settings.DepartureCheckInAdvanceDuration; // 始发站

                bool checking = now > departureTime - checkInStartOffset && now < departureTime - _settings.StopCheckInAdvanceDuration;

                if (checking)
                {
                    CheckingTrain = TrainInfo[0];
                    FirstTrain = TrainInfo.ElementAtOrDefault(1);
                    SecondTrain = TrainInfo.ElementAtOrDefault(2);
                }
                else
                {
                    CheckingTrain = null;
                    if(now < departureTime && now > departureTime - _settings.StopCheckInAdvanceDuration)
                    {
                        FirstTrain = TrainInfo.ElementAtOrDefault(1);
                        SecondTrain = TrainInfo.ElementAtOrDefault(2);
                        return;
                    }
                    FirstTrain = TrainInfo.ElementAtOrDefault(0);
                    SecondTrain = TrainInfo.ElementAtOrDefault(1);
                }
            });
        }
    }
}
