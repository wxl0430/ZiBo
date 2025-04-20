using CRSim.ScreenSimulator.Models;

namespace CRSim.ViewModels;

public partial class ScreenSimulationPageViewModel : ObservableObject 
{
    [ObservableProperty]
    private string _pageTitle = "引导屏模拟";

    [ObservableProperty]
    private string _pageDescription = "";

    [ObservableProperty]
    private ICollection<StylesInfoDataItem> _styleCards = StylesInfoDataSource.Instance.StylesInfo;

    private Type selectedStyle;

    [ObservableProperty]
    private string _selectedStyleName = "";

    [ObservableProperty]
    private bool _stationNeeded;

    [ObservableProperty]
    private bool _ticketCheckNeeded;

    [ObservableProperty]
    private bool _platformNeeded;

    [ObservableProperty]
    private bool _textNeeded;

    [ObservableProperty]
    private bool _videoNeeded;

    [ObservableProperty]
    private bool _locationNeeded;

    [ObservableProperty]
    private string _text = "";

    [ObservableProperty]
    private string _video = "";

    public string SelectedStationName = "";

    public string SelectedTicketCheck = "";

    public string SelectedPlatformName = "";

    public int SelectedLoaction = 0;

    public ObservableCollection<string> StationNames { get; private set; } = [];
    public ObservableCollection<string> TicketChecks { get; private set; } = [];
    public ObservableCollection<Platform> Platforms { get; private set; } = [];
    public ObservableCollection<int> Locations { get; private set; } = [];

    [ObservableProperty]
    private bool _isStartSimulationAvailable = false;

    private readonly IDatabaseService _databaseService;

    private readonly IServiceProvider _serviceProvider;

    private readonly IDialogService _dialogService;
    public ScreenSimulationPageViewModel(IDatabaseService databaseService, IServiceProvider serviceProvider, IDialogService dialogService)
    {
        _databaseService = databaseService;
        _serviceProvider = serviceProvider;
        _dialogService = dialogService;
        foreach (Station station in _databaseService.GetAllStations())
        {
            StationNames.Add(station.Name);
        }
    }

    private void CheckCanStart()
    {
        IsStartSimulationAvailable = selectedStyle != null &&
            (!StationNeeded || !string.IsNullOrWhiteSpace(SelectedStationName)) &&
            (!TicketCheckNeeded || !string.IsNullOrWhiteSpace(SelectedTicketCheck)) &&
            (!LocationNeeded || SelectedLoaction != 0) &&
            (!PlatformNeeded || !string.IsNullOrWhiteSpace(SelectedPlatformName));
    }

    [RelayCommand]
    public void SelectStyle(object s)
    {
        if (s is Type style)
        {
            selectedStyle = style;
            SelectedStyleName = StylesInfoDataSource.Instance.GetStyleName(style);
            var neededParameters = StylesInfoDataSource.Instance.GetParametersInfo(style);
            StationNeeded = neededParameters.Contains("Station");
            TicketCheckNeeded = neededParameters.Contains("TicketCheck");
            PlatformNeeded = neededParameters.Contains("Platform");
            VideoNeeded = neededParameters.Contains("Video");
            TextNeeded = neededParameters.Contains("Text");
            LocationNeeded = neededParameters.Contains("Location");
        }
        CheckCanStart();
    }
    [RelayCommand]
    public void SelectStation(object s)
    {
        if(s is string v)
        {
            var station = _databaseService.GetStationByName(v);
            SelectedStationName = station.Name;
            SelectedPlatformName = "";
            SelectedTicketCheck = "";
            SelectedLoaction = 0;
            TicketChecks.Clear();
            foreach (var waitingArea in station.WaitingAreas)
            {
                foreach (string ticketCheck in waitingArea.TicketChecks)
                {
                    TicketChecks.Add($"{waitingArea.Name} - {ticketCheck}");//重复检票口名称的临时解决方案
                }
            }
            Platforms.Clear();
            foreach (var platform in station.Platforms)
            {
                Platforms.Add(platform);
            }
            Locations.Clear();
        }
        CheckCanStart();
    }
    [RelayCommand]
    public void SelectTicketCheck(object s)
    {
        if (s is string ticketCheck)
        {
            SelectedTicketCheck = ticketCheck;
        }
        CheckCanStart();
    }
    [RelayCommand]
    public void SelectLocation(object s)
    {
        if (s is int location)
        {
            SelectedLoaction = location;
        }
        CheckCanStart();
    }
    [RelayCommand]
    public void SelectPlatform(object s)
    {
        if (s is Platform platform)
        {
            SelectedPlatformName = platform.Name;
            SelectedLoaction = 0;
            Locations.Clear();
            for (int i = 1; i <= platform.Length; i++)
            {
                Locations.Add(i);
            }
        }
        CheckCanStart();
    }
    [RelayCommand]
    public void SelectVideo()
    {
        string? Path = _dialogService.GetFile("视频文件 (*.mp4)|*.mp4|所有文件 (*.*)|*.*");
        if (Path == null) return;
        Video = Path;
    }
    [RelayCommand]
    public void StartSimulation()
    {
        var Window = _serviceProvider.GetRequiredService(selectedStyle) as dynamic;

        if (TextNeeded && Text != string.Empty)
        {
            Window.ViewModel.Text = Text;
        }
        if (LocationNeeded && SelectedLoaction != 0)
        {
            Window.ViewModel.Location = SelectedLoaction;
        }
        if (VideoNeeded && Video != string.Empty)
        {
            Window.ViewModel.Video = new Uri(Video, UriKind.Absolute);
        }

        Window.ViewModel.LoadData(_databaseService.GetStationByName(SelectedStationName),
            TicketCheckNeeded ? SelectedTicketCheck : string.Empty,
            PlatformNeeded ? SelectedPlatformName : string.Empty);
        Window.Show();
    }
    #region StyleSearch
    private string _keyWord = string.Empty;
    private string _station = string.Empty;
    private string _type = string.Empty;
    private string _author = string.Empty;
    public List<string> StyleStations { get; private set; } = [.. new[] { "全部" }.Concat(StylesInfoDataSource.Instance.StylesInfo.Select(x => x.Station).Distinct())];
    public List<string> StyleTypes { get; private set; } = [.. new[] { "全部" }.Concat(StylesInfoDataSource.Instance.StylesInfo.Select(x => x.Type).Distinct())];
    public List<string> StyleAuthors { get; private set; } = [.. new[] { "全部" }.Concat(StylesInfoDataSource.Instance.StylesInfo.Select(x => x.Author).Distinct())];
    
    [RelayCommand]
    public void TextSearch(object s)
    {
        if(s is string str)
        {
            _keyWord = str;
            Search();
        }
    }
    [RelayCommand]
    public void StationSearch(object s)
    {
        if (s is string str)
        {
            _station = str;
            Search();
        }
    }
    [RelayCommand]
    public void TypeSearch(object s)
    {
        if (s is string str)
        {
            _type = str;
            Search();
        }
    }
    [RelayCommand]
    public void AuthorSearch(object s)
    {
        if (s is string str)
        {
            _author = str;
            Search();
        }
    }
    private void Search()
    {
        StyleCards = [.. StylesInfoDataSource.Instance.StylesInfo.Where(x =>(x.Station==_station||_station==string.Empty||_station=="全部") && (x.Type==_type||_type==string.Empty||_type=="全部") && (x.Author==_author||_author==string.Empty||_author=="全部") && x.Title.Contains(_keyWord))];
    }
    #endregion
}
