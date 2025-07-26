using CRSim.Core.Models.Plugin;
using System.Diagnostics.CodeAnalysis;

namespace CRSim.ViewModels;

public partial class ScreenSimulatorPageViewModel(IServiceProvider serviceProvider, IDatabaseService databaseService) : ObservableObject
{
    public string PageTitle = "引导屏模拟";
    public List<PluginInfo> StyleInfos => StyleManager.StyleInfos;
    [ObservableProperty]
    public partial PluginInfo SelectedStyle { get; set; }

    [ObservableProperty]
    public partial bool StationNeeded { get; set; } = true;

    [ObservableProperty]
    public partial bool TicketCheckNeeded { get; set; }

    [ObservableProperty]
    public partial bool PlatformNeeded { get; set; } 

    [ObservableProperty]
    public partial bool TextNeeded { get; set; }

    [ObservableProperty]
    public partial bool VideoNeeded { get; set; }

    [ObservableProperty]
    public partial bool LocationNeeded { get; set; }
    public List<string> StationNames => [.. databaseService.GetAllStations().Select(x => x.Name)];
    public ObservableCollection<string> TicketChecks { get; private set; } = [];
    public ObservableCollection<Platform> Platforms { get; private set; } = [];
    public ObservableCollection<int> Locations { get; private set; } = [];
    [ObservableProperty]
    public partial string Text { get; set; }

    [ObservableProperty]
    public partial string Video { get; set; }

    public string SelectedStationName = "";

    public string SelectedTicketCheck = "";

    public string SelectedPlatformName = "";

    public int SelectedLoaction = 0;
    [ObservableProperty]
    public partial bool IsStartSimulationAvailable { get; private set; } = false;
    private PluginBase SelectedStylePlugin => serviceProvider.GetServices<PluginBase>().Where(x => x.Info == SelectedStyle).FirstOrDefault();

    [RelayCommand]
    public void StyleSelected(object selectedStyle)
    {
        if(selectedStyle is PluginInfo style)
        {
            SelectedStyle = style;
            var paramaters = style.StyleInfo?.Parameters;
            StationNeeded = paramaters.Contains("station");
            TicketCheckNeeded = paramaters.Contains("ticketCheck");
            PlatformNeeded = paramaters.Contains("platform");
            TextNeeded = paramaters.Contains("text");
            VideoNeeded = paramaters.Contains("video");
            LocationNeeded = paramaters.Contains("location");
        }
    }
    [RelayCommand]
    public void StationSelected(object s)
    {
        if (s is string v)
        {
            var station = databaseService.GetStationByName(v);
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
        //string? Path = _dialogService.GetFile("视频文件 (*.mp4)|*.mp4|所有文件 (*.*)|*.*");
        //if (Path == null) return;
        //Video = Path;
    }
    [RelayCommand]
    public async Task StartSimulation()
    {
        StyleManager.ShowWindow(SelectedStylePlugin.View, 
            databaseService.GetStationByName(SelectedStationName),
            TicketCheckNeeded ? SelectedTicketCheck : string.Empty,
            PlatformNeeded ? SelectedPlatformName : string.Empty,
            (TextNeeded && Text != string.Empty) ? Text : null,
            (LocationNeeded && SelectedLoaction != 0) ? SelectedLoaction : 0,
            (VideoNeeded && Video != string.Empty) ? Video : null);
    }

    private void CheckCanStart()
    {
        IsStartSimulationAvailable = SelectedStyle != null &&
            (!StationNeeded || !string.IsNullOrWhiteSpace(SelectedStationName)) &&
            (!TicketCheckNeeded || !string.IsNullOrWhiteSpace(SelectedTicketCheck)) &&
            (!LocationNeeded || SelectedLoaction != 0) &&
            (!PlatformNeeded || !string.IsNullOrWhiteSpace(SelectedPlatformName));
    }
}