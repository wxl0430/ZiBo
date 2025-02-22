namespace CRSim.ViewModels.Hanzhong
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        private ITimeService _timeService;
        public ObservableCollection<TrainInfo> Screen { get; set; } = [];
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            _timeService = timeService;
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
                Screen.Add(TrainInfo.Count > 0 ? TrainInfo[0] : new());
                if (Screen.Count == 1) return;
                Screen.RemoveAt(0);
            });
        }
        public override void RefreshData(object? sender, EventArgs e)
        {
            if (CurrentPageIndex == 0)
            {
                List<TrainInfo> itemsToRemove = [.. TrainInfo.Where(info => info.DepartureTime < _timeService.GetDateTimeNow())];
                foreach (var item in itemsToRemove)
                {
                    TrainInfo.Remove(item);
                }
            }
        }
    }
}
