namespace CRSim.ViewModels.ChengduDong
{
    public class SecondaryScreenViewModel : ScreenViewModel
    {
        public ObservableCollection<TrainInfo> LeftScreen { get; private set; } = [];
        public ObservableCollection<TrainInfo> RightScreen { get; private set; } = [];
        public SecondaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "    检票口信息请以车站显示屏信息为准。公共场所请妥善保管个人贵重物品。";
            ItemsPerPage = 12;
            PageCount = 2;
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
                RightScreen.Clear();
                int startIndex = CurrentPageIndex * ItemsPerPage * PageCount;
                var leftItems = TrainInfo.Skip(startIndex).Take(ItemsPerPage).ToList();
                while (leftItems.Count < ItemsPerPage) 
                {
                    leftItems.Add(new TrainInfo());
                }
                foreach (var item in leftItems)
                {
                    LeftScreen.Add(item);
                }
                var rightItems = TrainInfo.Skip(startIndex + ItemsPerPage).Take(ItemsPerPage).ToList();
                while (rightItems.Count < ItemsPerPage) 
                {
                    rightItems.Add(new TrainInfo());
                }
                foreach (var item in rightItems)
                {
                    RightScreen.Add(item);
                }
                CurrentPageIndex = CurrentPageIndex + 1 >=  Math.Min(_settings.MaxPages, pageCount) ? 0 : CurrentPageIndex + 1;
            });
        }
    }
}
