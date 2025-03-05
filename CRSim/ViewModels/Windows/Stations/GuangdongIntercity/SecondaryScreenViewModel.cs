namespace CRSim.ViewModels.GuangdongIntercity
{
    public class SecondaryScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> Screen { get; private set; } = [];
        public SecondaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 8;
            PageCount = 1;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            await WaitForDataLoadAsync();
            Text = $"列 车 到 发 信 息";
            RefreshDisplay(null,null);
        }
        private void RefreshDisplay(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                int pageCount = (int)Math.Ceiling((double)TrainInfo.Count / (ItemsPerPage * PageCount));
                Screen.Clear();
                int startIndex = CurrentPageIndex * ItemsPerPage * PageCount;
                var Items = TrainInfo.Skip(startIndex).Take(ItemsPerPage).ToList();
                while (Items.Count < ItemsPerPage) // Fill up with new object() if not enough items
                {
                    Items.Add(new TrainInfo());
                }
                foreach (var item in Items)
                {
                    Screen.Add(item);
                }

                CurrentPageIndex = CurrentPageIndex + 1 >= Math.Min(_settings.MaxPages, pageCount) ? 0 : CurrentPageIndex + 1;
            });
        }
    }
}
