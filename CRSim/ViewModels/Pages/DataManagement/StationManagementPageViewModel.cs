using CRSim.Core.Models;
using System.IO;
using System.Linq;
using System.Text;

namespace CRSim.ViewModels;

public partial class StationManagementPageViewModel : ObservableObject 
{
    public class TicketCheck
    {
        public string WaitingAreaName { get; set; }
        public string Name { get; set; }
    }
    [ObservableProperty]
	private string _pageTitle = "车站管理";
    
	[ObservableProperty]
	private string _pageDescription = "看起来不是很温馨的提醒：改完记得保存! Owo";

    [ObservableProperty]
    private bool _isSelected = false;

    [ObservableProperty]
    private List<string> _stationNames = [];

    [ObservableProperty]
    private Station _selectedStation = new();

    public ObservableCollection<string> WaitingAreaNames { get; private set; } = [];
    public ObservableCollection<string> Platforms { get; private set; } = [];

    public ObservableCollection<TicketCheck> TicketChecks { get; private set; } = [];

    public ObservableCollection<StationStop> StationStops { get; private set; } = [];

    private readonly IDatabaseService _databaseService;

    private readonly IDialogService _dialogService;

    private readonly INetworkService _networkService;
    public StationManagementPageViewModel(IDatabaseService databaseService,IDialogService dialogService,INetworkService networkService)
    {
		_databaseService = databaseService;
        _dialogService = dialogService;
        _networkService = networkService;
        RefreshStationList();
    }

	public void RefreshStationList()
    {
        var stationsList = _databaseService.GetAllStations();
        StationNames = [.. stationsList.Select(s => s.Name)];
    }

    [RelayCommand]
    public void StationSelected(object selectedStation)
    {
        WaitingAreaNames.Clear();
        Platforms.Clear();
        TicketChecks.Clear();
        StationStops.Clear();
        if(selectedStation is string selectedStationName)
        {
            SelectedStation = _databaseService.GetStationByName(selectedStationName);
            foreach(WaitingArea waitingArea in SelectedStation.WaitingAreas)
            {
                WaitingAreaNames.Add(waitingArea.Name);
                foreach (string ticketCheck in waitingArea.TicketChecks)
                {
                    TicketChecks.Add(new TicketCheck { WaitingAreaName = waitingArea.Name,Name=ticketCheck });
                }
            }
            foreach (StationStop stationStop in SelectedStation.StationStops)
            {
                StationStops.Add(stationStop);
            }
            foreach(string platform in SelectedStation.Platforms)
            {
                Platforms.Add(platform);
            }
            IsSelected = true;
        }
        if(selectedStation == null)
        {
            IsSelected = false;
        }
    }

    [RelayCommand]
    public void AddTicketCheck()
    {
        (string waitingAreaName,List<string> ticketChecks) = _dialogService.GetInputTicketCheck([.. WaitingAreaNames]);
        if (ticketChecks != null && waitingAreaName != null)
        {
            foreach(string ticketCheck in ticketChecks)
            {
                if (TicketChecks.Select(x => x.Name).Contains(ticketCheck))
                {
                    _dialogService.ShowMessage("添加失败", $"编号 {ticketCheck} 检票口已存在。");
                    return;
                }
                TicketChecks.Add(new TicketCheck { WaitingAreaName = waitingAreaName, Name = ticketCheck });
            }
        }
    }

    [RelayCommand]
    public void DeleteTicketCheck(object selectedTicketCheck)
    {
        if(selectedTicketCheck is TicketCheck ticketCheck)
        {
            TicketChecks.Remove(ticketCheck);
        }
    }
    [RelayCommand]
    public void DeleteAllTicketCheck()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部检票口，是否继续？")) return;
        TicketChecks.Clear();
    }
    [RelayCommand]
    public void AddWaitingArea()
    {
        string? waitingAreaName = _dialogService.GetInput("请输入候车室名称");
        if (waitingAreaName != null)
        {
            if (WaitingAreaNames.Contains(waitingAreaName))
            {
                _dialogService.ShowMessage("添加失败",$"名称 {waitingAreaName} 候车室已存在。");
                return;
            }
            WaitingAreaNames.Add(waitingAreaName);
        }
    }

    [RelayCommand]
    public void DeleteWaitingArea(object selectedWaitingArea)
    {
        if(selectedWaitingArea is string n)
        {
            WaitingAreaNames.Remove(n);
        }
    }
    [RelayCommand]
    public void DeleteAllWaitingArea()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部候车室，是否继续？")) return;
        WaitingAreaNames.Clear();
    }
    [RelayCommand]
    public void AddPlatform()
    {
        string? platform = _dialogService.GetInput("请输入站台名称");
        if (platform != null)
        {
            string[] platforms = platform.Split('-');
            if (platforms.Length == 2 && int.TryParse(platforms[0],out int startindex) && int.TryParse(platforms[1], out int endindex))
            {
                var startIndex = Math.Min(startindex, endindex);
                var endIndex = Math.Max(startindex, endindex);
                for(int i = startIndex; i < endIndex + 1; i++)
                {
                    if (Platforms.Contains(i.ToString()))
                    {
                        _dialogService.ShowMessage("添加失败", $"名称 {i} 站台已存在。");
                        return;
                    }
                    Platforms.Add(i.ToString());
                }
            }
            else
            {
                if (Platforms.Contains(platform))
                {
                    _dialogService.ShowMessage("添加失败", $"名称 {platform} 站台已存在。");
                    return;
                }
                Platforms.Add(platform);
            }
        }
    }

    [RelayCommand]
    public void DeletePlatform(object selectedPlatform)
    {
        if (selectedPlatform is string n)
        {
            Platforms.Remove(n);
        }
    }
    [RelayCommand]
    public void DeleteAllPlatform()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部站台，是否继续？")) return;
        Platforms.Clear();
    }

    [RelayCommand]
    public async Task AddStation()
    {
        string? station = _dialogService.GetInput("请输入车站名称");
        if (station != null)
        {
            if (StationNames.Contains(station))
            {
                _dialogService.ShowMessage("添加失败", $"车站 {station} 已存在。");
                return;
            }
            _databaseService.AddStationByName(station);
            await _databaseService.SaveData();
            RefreshStationList();
        }
    }

    [RelayCommand]
    public async Task DeleteStation(object selectedStation)
    {
        if (selectedStation is string s && !string.IsNullOrEmpty(s))
        {
            _databaseService.DeleteStation(_databaseService.GetStationByName(s));
            await _databaseService.SaveData();
            RefreshStationList();
        }
    }
    [RelayCommand]
    public async Task DeleteAllStations()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部车站，是否继续？")) return;
        foreach(string stationName in StationNames)
        {
            _databaseService.DeleteStation(_databaseService.GetStationByName(stationName));
        }
        await _databaseService.SaveData();
        RefreshStationList();
    }
    [RelayCommand]
    public async Task SaveChanges()
    {
        _databaseService.UpdateStation(SelectedStation.Name, GenerateStation(SelectedStation.Name,WaitingAreaNames,TicketChecks,StationStops,Platforms));
        await _databaseService.SaveData();
    }
    public static Station GenerateStation(string stationName, ObservableCollection<string> waitingAreaNames, ObservableCollection<TicketCheck> ticketChecks,ObservableCollection<StationStop> stationStops,ObservableCollection<string> platforms)
    {
        var station = new Station
        {
            Name = stationName,
            WaitingAreas = [.. waitingAreaNames.Select(name => new WaitingArea { Name = name })],
            StationStops = [.. stationStops],
            Platforms = [..platforms]
        };

        // 将 TicketCheck 映射到对应的 WaitingArea
        foreach (var ticketCheck in ticketChecks)
        {
            var waitingArea = station.WaitingAreas.FirstOrDefault(wa => wa.Name == ticketCheck.WaitingAreaName);
            waitingArea?.TicketChecks.Add(ticketCheck.Name);
        }

        return station;
    }
    [RelayCommand]
    public async Task ImportFromTimeTable()
    {
        List<string> stationNamesToAdd = [];
        foreach(TrainNumber trainNumber in _databaseService.GetAllTrainNumbers())
        {
            var stationNames = trainNumber.TimeTable.Select(x => x.Station).ToList();
            foreach(string stationName in stationNames)
            {
                if(!stationNamesToAdd.Contains(stationName) && !StationNames.Contains(stationName))
                {
                    stationNamesToAdd.Add(stationName);
                }
            }
        }
        foreach(string stationName in stationNamesToAdd)
        {
            _databaseService.AddStationByName(stationName);
        }
        await SaveChanges();
        RefreshStationList();
    }
    [RelayCommand]
    public async Task ImportFromLulutong()
    {
        if (TicketChecks.Count == 0)
        {
            _dialogService.ShowMessage("导入失败", "请先添加检票口。");
            return;
        }
        if (Platforms.Count == 0)
        {
            _dialogService.ShowMessage("导入失败", "请先添加站台。");
            return;
        }
        var path = _dialogService.GetFile("CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*");
        if (path == null) return;
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var lines = new List<string>();
            var reader = new StreamReader(path, Encoding.GetEncoding("GBK"));
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                lines.Add(line);
            }
            foreach (var line in lines.Skip(13))
            {
                var data = line.Split(",");
                var firstColumn = data[0].Trim();
                if (!string.IsNullOrEmpty(firstColumn) && char.IsLetterOrDigit(firstColumn[0]))
                {
                    int spaceIndex = firstColumn.IndexOf(' ');
                    if (spaceIndex != -1)
                    {
                        firstColumn = firstColumn[..spaceIndex];
                    }
                    if (StationStops.Any(x=> x.Number==firstColumn)) continue;
                    DateTime? departureTime = DateTime.Parse(data[4]);
                    DateTime? arrivalTime = departureTime.Value.Subtract(TimeSpan.FromMinutes(2));
                    if (data[1].Split("-")[0] == SelectedStation.Name)
                    {
                        arrivalTime = null;
                    }
                    else if (data[1].Split("-")[1] == SelectedStation.Name)
                    {
                        departureTime = null;
                    }
                    StationStops.Add(new StationStop()
                    { 
                        Number = firstColumn, 
                        Origin = data[1].Split("-")[0], 
                        Terminal = data[1].Split("-")[1], 
                        ArrivalTime = arrivalTime,
                        DepartureTime = departureTime, 
                        TicketChecks = [TicketChecks[new Random().Next(TicketChecks.Count)].Name],
                        Platform = Platforms[new Random().Next(Platforms.Count)],
                        Landmark = new[] { "红色", "绿色", "褐色", "蓝色", "紫色", "黄色", "橙色",null}[new Random().Next(8)]
                    });
                }
            }
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    [RelayCommand]
    public async Task ImportFrom7D()
    {
        var path = _dialogService.GetFile("可执行程序文件 (*.exe)|*.exe|所有文件 (*.*)|*.*");
        if (path == null) return;
        try
        {
            await _databaseService.ImportStationFrom7D(path);
            await _databaseService.SaveData();
            RefreshStationList();
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    [RelayCommand]
    public void AddStationStop()
    {
        var newStationStop = _dialogService.GetInputStationStop([.. TicketChecks.Select(x => x.Name)], [.. Platforms]);
        if (newStationStop != null)
        {
            if (StationStops.Any(x => x.Number == newStationStop.Number))
            {
                _dialogService.ShowMessage("添加失败", $"车次 {newStationStop.Number} 已存在。");
                return;
            }
            StationStops.Add(newStationStop);
        }
    }
    [RelayCommand]
    public void DeleteStationStop(object selectedStationStop)
    {
        if (selectedStationStop is StationStop s)
        {
            StationStops.Remove(s);
        }
    }
    [RelayCommand]
    public void DeleteAllStationStop()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部车次，是否继续？")) return;
        StationStops.Clear();
    }
    [RelayCommand]
    public void EditStationStop(object _selectedStationStop)
    {
        if (_selectedStationStop is StationStop selectedStationStop)
        {
            var newStationStop = _dialogService.GetInputStationStop([.. TicketChecks.Select(x => x.Name)], [.. Platforms], selectedStationStop);
            if (newStationStop != null)
            {
                StationStops[StationStops.IndexOf(selectedStationStop)] = newStationStop;
            }
        }
    }
    [RelayCommand]
    public async Task ImportFromInternet()
    {
        if (TicketChecks.Count == 0)
        {
            _dialogService.ShowMessage("导入失败", "请先添加检票口。");
            return;
        }
        if (Platforms.Count == 0)
        {
            _dialogService.ShowMessage("导入失败", "请先添加站台。");
            return;
        }
        if (StationStops.Count != 0)
        {
            if (!_dialogService.GetConfirm("当前操作会清空时刻表。是否继续？"))
            {
                return;
            }
        }

        var stops = await _networkService.GetStationStopsAsync(SelectedStation.Name);
        if (stops.Count !=0)
        {
            StationStops.Clear();
            foreach (StationStop t in stops)
            {
                t.TicketChecks = [TicketChecks[new Random().Next(TicketChecks.Count)].Name];
                t.Platform = Platforms[new Random().Next(Platforms.Count)];
                t.Landmark = new[] { "红色", "绿色", "褐色", "蓝色", "紫色", "黄色", "橙色", null }[new Random().Next(8)];
                StationStops.Add(t);
            }
        }
        else
        {
            _dialogService.ShowMessage("获取失败", "服务器繁忙或车站不存在。");
        }
    }

}
