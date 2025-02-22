namespace CRSim.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _pageTitle = "设置";
        [ObservableProperty]
        private string _appVersion = "";
        private readonly Settings _settings;
        private readonly ISettingsService _settingsService;
        private readonly IDatabaseService _databaseService;
        private readonly IDialogService _dialogService;

        public SettingsPageViewModel(ISettingsService settingsService, IDatabaseService databaseService, IDialogService dialogService)
        {
            _settings = settingsService.GetSettings();
            _settingsService = settingsService;
            _databaseService = databaseService;
            _dialogService = dialogService;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = $"CRSim v{version}";
            _dialogService = dialogService;
        }
        #region 偏好设置
        public string TimeOffset
        {
            get => _settings.TimeOffset.TotalMinutes.ToString();
            set
            {
                if (double.TryParse(value, out double minutes))
                {
                    _settings.TimeOffset = TimeSpan.FromMinutes(minutes);
                    OnPropertyChanged(nameof(TimeOffset));
                    _settingsService.SaveSettings();
                }
            }
        }
        public string DepartureCheckInAdvanceDuration
        {
            get => _settings.DepartureCheckInAdvanceDuration.TotalMinutes.ToString();
            set
            {
                if (int.TryParse(value, out int minutes))
                {
                    _settings.DepartureCheckInAdvanceDuration = TimeSpan.FromMinutes(minutes);
                    OnPropertyChanged(nameof(DepartureCheckInAdvanceDuration));
                    _settingsService.SaveSettings();
                }
            }
        }

        public string PassingCheckInAdvanceDuration
        {
            get => _settings.PassingCheckInAdvanceDuration.TotalMinutes.ToString();
            set
            {
                if (int.TryParse(value, out int minutes))
                {
                    _settings.PassingCheckInAdvanceDuration = TimeSpan.FromMinutes(minutes);
                    OnPropertyChanged(nameof(PassingCheckInAdvanceDuration));
                    _settingsService.SaveSettings();
                }
            }
        }

        public string StopDisplayUntilDepartureDuration
        {
            get => _settings.StopDisplayUntilDepartureDuration.TotalMinutes.ToString();
            set
            {
                if (int.TryParse(value, out int minutes))
                {
                    _settings.StopDisplayUntilDepartureDuration = TimeSpan.FromMinutes(minutes);
                    OnPropertyChanged(nameof(StopDisplayUntilDepartureDuration));
                    _settingsService.SaveSettings();
                }
            }
        }

        public string StopCheckInAdvanceDuration
        {
            get => _settings.StopCheckInAdvanceDuration.TotalMinutes.ToString();
            set
            {
                if (int.TryParse(value, out int minutes))
                {
                    _settings.StopCheckInAdvanceDuration = TimeSpan.FromMinutes(minutes);
                    OnPropertyChanged(nameof(StopCheckInAdvanceDuration));
                    _settingsService.SaveSettings();
                }
            }
        }

        public string MaxPages
        {
            get => _settings.MaxPages.ToString();
            set
            {
                if (int.TryParse(value, out int maxPages))
                {
                    _settings.MaxPages = maxPages;
                    OnPropertyChanged(nameof(MaxPages));
                    _settingsService.SaveSettings();
                }
            }
        }

        public string SwitchPageSeconds
        {
            get => _settings.SwitchPageSeconds.ToString();
            set
            {
                if (int.TryParse(value, out int switchPageSeconds))
                {
                    _settings.SwitchPageSeconds = switchPageSeconds;
                    OnPropertyChanged(nameof(SwitchPageSeconds));
                    _settingsService.SaveSettings();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
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
