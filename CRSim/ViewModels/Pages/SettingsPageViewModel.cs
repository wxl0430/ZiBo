namespace CRSim.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _pageTitle = "设置";
        [ObservableProperty]
        private string _appVersion = "";
        private Settings _settings;
        private readonly ISettingsService _settingsService;
        private readonly IDatabaseService _databaseService;
        private readonly IDialogService _dialogService;
        #region 偏好设置
        [ObservableProperty]
        public string _timeOffset;
        [ObservableProperty]
        public string _departureCheckInAdvanceDuration;
        [ObservableProperty]
        public string _passingCheckInAdvanceDuration;
        [ObservableProperty]
        public string _stopDisplayUntilDepartureDuration;
        [ObservableProperty]
        public string _stopDisplayFromArrivalDuration;
        [ObservableProperty]
        public string _stopCheckInAdvanceDuration;
        [ObservableProperty]
        public string _maxPages;
        [ObservableProperty]
        public string _switchPageSeconds;
        [ObservableProperty]
        public string _userKey;
        [ObservableProperty]
        public bool _loadTodayOnly;
        [ObservableProperty]
        public bool _reopenUnclosedScreensOnLoad;
        #endregion
        public SettingsPageViewModel(ISettingsService settingsService, IDatabaseService databaseService, IDialogService dialogService)
        {
            LoadSettings(settingsService);
            _settingsService = settingsService;
            _databaseService = databaseService;
            _dialogService = dialogService;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = $"CRSim v{version}";
            _dialogService = dialogService;
        }

        private void LoadSettings(ISettingsService settingsService)
        {
            _settings = settingsService.GetSettings();
            TimeOffset = _settings.TimeOffset.TotalMinutes.ToString();
            DepartureCheckInAdvanceDuration = _settings.DepartureCheckInAdvanceDuration.TotalMinutes.ToString();
            PassingCheckInAdvanceDuration = _settings.PassingCheckInAdvanceDuration.TotalMinutes.ToString();
            StopDisplayUntilDepartureDuration = _settings.StopDisplayUntilDepartureDuration.TotalMinutes.ToString();
            StopDisplayFromArrivalDuration = _settings.StopDisplayFromArrivalDuration.TotalMinutes.ToString();
            StopCheckInAdvanceDuration = _settings.StopCheckInAdvanceDuration.TotalMinutes.ToString();
            MaxPages = _settings.MaxPages.ToString();
            SwitchPageSeconds = _settings.SwitchPageSeconds.ToString();
            UserKey = _settings.UserKey;
            LoadTodayOnly = _settings.LoadTodayOnly;
            ReopenUnclosedScreensOnLoad = _settings.ReopenUnclosedScreensOnLoad;
        }

        [RelayCommand]
        public void Unload()
        {
            UpdateSettings(TimeOffset, true, value => _settings.TimeOffset = TimeSpan.FromMinutes(value));
            UpdateSettings(DepartureCheckInAdvanceDuration, false, value => _settings.DepartureCheckInAdvanceDuration = TimeSpan.FromMinutes(value));
            UpdateSettings(PassingCheckInAdvanceDuration, false, value => _settings.PassingCheckInAdvanceDuration = TimeSpan.FromMinutes(value));
            UpdateSettings(StopDisplayUntilDepartureDuration, false, value => _settings.StopDisplayUntilDepartureDuration = TimeSpan.FromMinutes(value));
            UpdateSettings(StopDisplayFromArrivalDuration, false, value => _settings.StopDisplayFromArrivalDuration = TimeSpan.FromMinutes(value));
            UpdateSettings(StopCheckInAdvanceDuration, false, value => _settings.StopCheckInAdvanceDuration = TimeSpan.FromMinutes(value));
            UpdateSettings(MaxPages, false, value => _settings.MaxPages = value);
            UpdateSettings(SwitchPageSeconds, false, value => _settings.SwitchPageSeconds = value);
            _settings.UserKey = UserKey;
            _settings.LoadTodayOnly = LoadTodayOnly;
            _settings.ReopenUnclosedScreensOnLoad = ReopenUnclosedScreensOnLoad;
            _settingsService.SaveSettings();
        }
        private void UpdateSettings(string input,bool allowNegative, Action<int> updateAction)
        {
            if (int.TryParse(input, out int value) && (allowNegative || value >= 0))
            {
                updateAction(value);
            }
        }

        [RelayCommand]
        public async Task ClearData()
        {
            if (!_dialogService.GetConfirm("该操作将会清空所有数据，请否继续？")) return;
            _databaseService.ClearData();
            await _databaseService.SaveData();
        }
        [RelayCommand]
        public async Task ExportData()
        {
            string? path = _dialogService.SaveFile("JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*","data");
            if (string.IsNullOrWhiteSpace(path)) return;
            await _databaseService.ExportData(path);
        }
        [RelayCommand]
        public async Task ImportData()
        {
            string? path = _dialogService.GetFile("JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*");
            if (string.IsNullOrWhiteSpace(path)) return;
            _databaseService.ImportData(path);
            await _databaseService.SaveData();
        }
    }
}
