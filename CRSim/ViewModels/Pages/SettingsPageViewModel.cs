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
        }

        [RelayCommand]
        public void Unload()
        {
            if (int.TryParse(_timeOffset,out int i))
            {
                _settings.TimeOffset = TimeSpan.FromMinutes(i);
            }
            if (int.TryParse(DepartureCheckInAdvanceDuration, out int j) && j >= 0)
            {
                _settings.DepartureCheckInAdvanceDuration = TimeSpan.FromMinutes(j);
            }
            if (int.TryParse(PassingCheckInAdvanceDuration, out int k) && k >= 0)
            {
                _settings.PassingCheckInAdvanceDuration = TimeSpan.FromMinutes(k);
            }
            if (int.TryParse(StopDisplayUntilDepartureDuration, out int l) && l >= 0)
            {
                _settings.StopDisplayUntilDepartureDuration = TimeSpan.FromMinutes(l);
            }
            if (int.TryParse(StopDisplayFromArrivalDuration, out int p) && p >= 0)
            {
                _settings.StopDisplayFromArrivalDuration = TimeSpan.FromMinutes(p);
            }
            if (int.TryParse(StopCheckInAdvanceDuration, out int m) && m >= 0)
            {
                _settings.StopCheckInAdvanceDuration = TimeSpan.FromMinutes(m);
            }
            if (int.TryParse(MaxPages, out int n) && n >= 0)
            {
                _settings.MaxPages = n;
            }
            if (int.TryParse(SwitchPageSeconds, out int o) && o >= 0)
            {
                _settings.SwitchPageSeconds = o;
            }
            _settingsService.SaveSettings();
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
