using CRSim.Core.Models;
using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.Beijing
{
    public class ArrivalScreenViewModel : ScreenViewModel
    {
        private ITimeService _timeService;
        public ObservableCollection<TrainInfo> Screen { get; set; } = [];
        public ArrivalScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            _timeService = timeService;
            StationType = StationType.Arrival;
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
        public override void RefreshData(object? sender, EventArgs e)
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
