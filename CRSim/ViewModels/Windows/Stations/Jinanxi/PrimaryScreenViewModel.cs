namespace CRSim.ViewModels.Jinanxi
{
    public class PrimaryScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> Screen { get; private set; } = [];
        public PrimaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"济 南 西 站 欢 迎 您";
            ItemsPerPage = 9;
            PageCount = 1;
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
