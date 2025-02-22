namespace CRSim.ViewModels;

public partial class ScreenSimulationPageViewModel : ObservableObject 
{
	[ObservableProperty]
	private string _pageTitle = "Òýµ¼ÆÁÄ£Äâ";

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
    private string _text = "";

    public string SelectedStationName = "";

    public string SelectedTicketCheck = "";

    public string SelectedPlatform = "";

    public ObservableCollection<string> StationNames { get; private set; } = [];
    public ObservableCollection<string> TicketChecks { get; private set; } = [];
    public ObservableCollection<string> Platforms { get; private set; } = [];

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
        IsStartSimulationAvailable = selectedStyle!=null &&
            (!StationNeeded || !string.IsNullOrWhiteSpace(SelectedStationName))&&
            (!TicketCheckNeeded || !string.IsNullOrWhiteSpace(SelectedTicketCheck)) &&
            (!PlatformNeeded || !string.IsNullOrWhiteSpace(SelectedPlatform));
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
        }
        CheckCanStart();
    }
    [RelayCommand]
    public void SelectStation(object s)
    {
        if(s is string station)
        {
            SelectedStationName = station;
            TicketChecks.Clear();
            foreach(string ticketCheck in _databaseService.GetStationByName(SelectedStationName).TicketChecks)
            {
                TicketChecks.Add(ticketCheck);
            }
            Platforms.Clear();
            foreach (string platform in _databaseService.GetStationByName(SelectedStationName).Platforms)
            {
                Platforms.Add(platform);
            }
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
    public void SelectPlatform(object s)
    {
        if (s is string platform)
        {
            SelectedPlatform = platform;
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

        Window.ViewModel.LoadData(_databaseService.GetStationByName(SelectedStationName),
            TicketCheckNeeded ? SelectedTicketCheck : string.Empty,
            PlatformNeeded ? SelectedPlatform : string.Empty);
        Window.Show();
    }
}
