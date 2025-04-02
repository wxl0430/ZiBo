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
    private bool _locationNeeded;

    [ObservableProperty]
    private string _text = "";

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
            foreach(string ticketCheck in station.TicketChecks)
            {
                TicketChecks.Add(ticketCheck);
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

        Window.ViewModel.LoadData(_databaseService.GetStationByName(SelectedStationName),
            TicketCheckNeeded ? SelectedTicketCheck : string.Empty,
            PlatformNeeded ? SelectedPlatformName : string.Empty);
        Window.Show();
    }
}
