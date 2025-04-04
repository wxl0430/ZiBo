using CRSim.Core.Models;
using OfficeOpenXml;

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
    public ObservableCollection<Platform> Platforms { get; private set; } = [];

    public ObservableCollection<TicketCheck> TicketChecks { get; private set; } = [];

    public ObservableCollection<TrainStop> TrainStops { get; private set; } = [];

    private readonly IDatabaseService _databaseService;

    private readonly IDialogService _dialogService;

    private readonly INetworkService _networkService;
    public StationManagementPageViewModel(IDatabaseService databaseService,IDialogService dialogService,INetworkService networkService)
    {
		_databaseService = databaseService;
        _dialogService = dialogService;
        _networkService = networkService;
        RefreshStations();
    }

    #region Station
    public void RefreshStations()
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
        TrainStops.Clear();
        if (selectedStation is string selectedStationName)
        {
            SelectedStation = _databaseService.GetStationByName(selectedStationName);
            foreach (WaitingArea waitingArea in SelectedStation.WaitingAreas)
            {
                WaitingAreaNames.Add(waitingArea.Name);
                foreach (string ticketCheck in waitingArea.TicketChecks)
                {
                    TicketChecks.Add(new TicketCheck { WaitingAreaName = waitingArea.Name, Name = ticketCheck });
                }
            }
            foreach (TrainStop trainStop in SelectedStation.TrainStops)
            {
                TrainStops.Add(trainStop);
            }
            foreach (var platform in SelectedStation.Platforms)
            {
                Platforms.Add(platform);
            }
            IsSelected = true;
        }
        if (selectedStation == null)
        {
            IsSelected = false;
        }
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
            RefreshStations();
        }
    }
    [RelayCommand]
    public async Task DeleteStation(object selectedStation)
    {
        if (selectedStation is string s && !string.IsNullOrEmpty(s))
        {
            _databaseService.DeleteStation(_databaseService.GetStationByName(s));
            await _databaseService.SaveData();
            RefreshStations();
        }
    }
    [RelayCommand]
    public async Task DeleteAllStations()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部车站，是否继续？")) return;
        foreach (string stationName in StationNames)
        {
            _databaseService.DeleteStation(_databaseService.GetStationByName(stationName));
        }
        await _databaseService.SaveData();
        RefreshStations();
    }
    public static Station GenerateStation(string stationName, ObservableCollection<string> waitingAreaNames, ObservableCollection<TicketCheck> ticketChecks, ObservableCollection<TrainStop> trainStops, ObservableCollection<Platform> platforms)
    {
        var station = new Station
        {
            Name = stationName,
            WaitingAreas = [.. waitingAreaNames.Select(name => new WaitingArea { Name = name })],
            TrainStops = [.. trainStops],
            Platforms = [.. platforms]
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
    public async Task ImportFrom7D()
    {
        var path = _dialogService.GetFile("可执行程序文件 (*.exe)|*.exe|所有文件 (*.*)|*.*");
        if (path == null) return;
        try
        {
            await _databaseService.ImportStationFrom7D(path);
            await _databaseService.SaveData();
            RefreshStations();
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    #endregion

    #region TicketCheck
    [RelayCommand]
    public void AddTicketCheck()
    {
        (string waitingAreaName, List<string> ticketChecks) = _dialogService.GetInputTicketCheck([.. WaitingAreaNames]);
        if (ticketChecks != null && waitingAreaName != null)
        {
            foreach (string ticketCheck in ticketChecks)
            {
                if (TicketChecks.Where(x => x.WaitingAreaName == waitingAreaName).Select(x => x.Name).Contains(ticketCheck))
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
        if (selectedTicketCheck is TicketCheck ticketCheck)
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
    #endregion

    #region WaitingArea
    [RelayCommand]
    public void AddWaitingArea()
    {
        string? waitingAreaName = _dialogService.GetInput("请输入候车室名称");
        if (waitingAreaName != null)
        {
            if (WaitingAreaNames.Contains(waitingAreaName))
            {
                _dialogService.ShowMessage("添加失败", $"名称 {waitingAreaName} 候车室已存在。");
                return;
            }
            WaitingAreaNames.Add(waitingAreaName);
        }
    }
    [RelayCommand]
    public void DeleteWaitingArea(object selectedWaitingArea)
    {
        if (selectedWaitingArea is string n)
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
    #endregion

    #region Platform
    [RelayCommand]
    public void AddPlatform()
    {
        var platforms = _dialogService.GetInputPlatform();
        if (platforms != null)
        {
            foreach (var platform in platforms)
            {
                if (Platforms.Any(x => x.Name == platform.Name))
                {
                    _dialogService.ShowMessage("添加失败", $"编号 {platform.Name} 站台已存在。");
                    return;
                }
                Platforms.Add(platform);
            }
        }
    }
    [RelayCommand]
    public void DeletePlatform(object selectedPlatform)
    {
        if (selectedPlatform is Platform p)
        {
            Platforms.Remove(p);
        }
    }
    [RelayCommand]
    public void DeleteAllPlatform()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部站台，是否继续？")) return;
        Platforms.Clear();
    }
    #endregion

    #region TrainStop
    [RelayCommand]
    public void ImportFromTrainNumbers()
    {
        if (!CheckCanImport()) return;
        var trainNumbers = _databaseService.GetAllTrainNumbers();
        if (trainNumbers.Count != 0)
        {
            TrainStops.Clear();
        }
        foreach (TrainNumber trainNumber in trainNumbers)
        {
            var t = trainNumber.TimeTable.FirstOrDefault(x => x.Station == SelectedStation.Name);
            if (t == null) continue;
            t.Number = trainNumber.Number;
            t = RandomTrainStopProperties(t);
            t.Origin = trainNumber.TimeTable.First().Station;
            t.Terminal = trainNumber.TimeTable.Last().Station;
            TrainStops.Add(t);
        }
    }
    [RelayCommand]
    public async Task ImportFromLulutong()
    {
        if (!CheckCanImport()) return;
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
                    if (TrainStops.Any(x => x.Number == firstColumn)) continue;
                    TimeSpan? departureTime = TimeSpan.Parse(data[4]);
                    TimeSpan? arrivalTime = departureTime.Value.Subtract(TimeSpan.FromMinutes(2));
                    if (data[1].Split("-")[0] == SelectedStation.Name)
                    {
                        arrivalTime = null;
                    }
                    else if (data[1].Split("-")[1] == SelectedStation.Name)
                    {
                        departureTime = null;
                    }
                    var trainStop = new TrainStop()
                    {
                        Number = firstColumn,
                        Origin = data[1].Split("-")[0],
                        Terminal = data[1].Split("-")[1],
                        ArrivalTime = arrivalTime,
                        DepartureTime = departureTime,
                    };
                    TrainStops.Add(RandomTrainStopProperties(trainStop));
                }
            }
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    [RelayCommand]
    public void ImportFromExcel()
    {
        if (TrainStops.Count != 0)
        {
            if (!_dialogService.GetConfirm("当前操作可能会清空时刻表。是否继续？"))
            {
                return;
            }
        }
        var path = _dialogService.GetFile("Excel 工作薄 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*");
        if (path == null) return;
        try
        {
            ExcelPackage.License.SetNonCommercialPersonal("CRSim");
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            TrainStops.Clear();
            for (int row = 2; row <= rowCount; row++)
            {
                if (worksheet.Cells[row, 1].Text.Trim() == "" ||
                    worksheet.Cells[row, 2].Text.Trim() == "" ||
                    worksheet.Cells[row, 5].Text.Trim() == "" ||
                    worksheet.Cells[row, 6].Text.Trim() == "" ||
                    worksheet.Cells[row, 7].Text.Trim() == "" ||
                    worksheet.Cells[row, 9].Text.Trim() == "" ||
                    worksheet.Cells[row, 10].Text.Trim() == "")
                {
                    continue;
                }

                TrainStops.Add(new TrainStop
                {
                    Number = worksheet.Cells[row, 1].Text.Trim(),
                    Length = int.TryParse(worksheet.Cells[row, 2].Text, out int length) ? length : 0,
                    ArrivalTime = TimeSpan.TryParseExact(worksheet.Cells[row, 3].Text, @"hh\:mm", null, out TimeSpan arrival) ? arrival : null,
                    DepartureTime = TimeSpan.TryParseExact(worksheet.Cells[row, 4].Text, @"hh\:mm", null, out TimeSpan departure) ? departure : null,
                    WaitingArea = worksheet.Cells[row, 5].Text.Trim(),
                    TicketChecks = [.. worksheet.Cells[row, 6].Text.Split(' ', StringSplitOptions.RemoveEmptyEntries)],
                    Platform = worksheet.Cells[row, 7].Text.Trim(),
                    Landmark = worksheet.Cells[row, 8].Text.Trim() == "" ? "无" : worksheet.Cells[row, 8].Text.Trim(),
                    Origin = worksheet.Cells[row, 9].Text.Trim(),
                    Terminal = worksheet.Cells[row, 10].Text.Trim()
                });
            }
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    [RelayCommand]
    public void ExportToExcel()
    {
        var path = _dialogService.SaveFile("Excel 工作薄 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*", "data");
        if (path == null) return;
        ExcelPackage.License.SetNonCommercialPersonal("CRSim");
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        worksheet.Cells[1, 1].Value = "车次";
        worksheet.Cells[1, 2].Value = "长度";
        worksheet.Cells[1, 3].Value = "到时";
        worksheet.Cells[1, 4].Value = "发时";
        worksheet.Cells[1, 5].Value = "候车区";
        worksheet.Cells[1, 6].Value = "检票口";
        worksheet.Cells[1, 7].Value = "站台";
        worksheet.Cells[1, 8].Value = "地标";
        worksheet.Cells[1, 9].Value = "始发站";
        worksheet.Cells[1, 10].Value = "终到站";
        for (int i = 0; i < TrainStops.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = TrainStops[i].Number;
            worksheet.Cells[i + 2, 2].Value = TrainStops[i].Length;
            worksheet.Cells[i + 2, 3].Value = TrainStops[i].ArrivalTime?.ToString(@"hh\:mm") ?? string.Empty;
            worksheet.Cells[i + 2, 4].Value = TrainStops[i].DepartureTime?.ToString(@"hh\:mm") ?? string.Empty;
            worksheet.Cells[i + 2, 5].Value = TrainStops[i].WaitingArea;
            worksheet.Cells[i + 2, 6].Value = TrainStops[i].TicketChecks.Count == 0 ? string.Empty : string.Join(" ", TrainStops[i].TicketChecks);
            worksheet.Cells[i + 2, 7].Value = TrainStops[i].Platform;
            worksheet.Cells[i + 2, 8].Value = TrainStops[i].Landmark;
            worksheet.Cells[i + 2, 9].Value = TrainStops[i].Origin;
            worksheet.Cells[i + 2, 10].Value = TrainStops[i].Terminal;
        }
        package.SaveAs(new FileInfo(path));
    }
    [RelayCommand]
    public void AddTrainStop()
    {
        var newTrainStop = _dialogService.GetInputTrainStop([.. TicketChecks.Select(x => $"{x.WaitingAreaName} - {x.Name}")], [.. Platforms.Select(x => x.Name)]);
        if (newTrainStop != null)
        {
            if (TrainStops.Any(x => x.Number == newTrainStop.Number))
            {
                _dialogService.ShowMessage("添加失败", $"车次 {newTrainStop.Number} 已存在。");
                return;
            }
            TrainStops.Add(newTrainStop);
        }
    }
    [RelayCommand]
    public void DeleteTrainStop(object selectedTrainStop)
    {
        if (selectedTrainStop is TrainStop s)
        {
            TrainStops.Remove(s);
        }
    }
    [RelayCommand]
    public void DeleteAllTrainStop()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部车次，是否继续？")) return;
        TrainStops.Clear();
    }
    [RelayCommand]
    public void EditTrainStop(object _selectedTrainStop)
    {
        if (_selectedTrainStop is TrainStop selectedTrainStop)
        {
            var newTrainStop = _dialogService.GetInputTrainStop([.. TicketChecks.Select(x => $"{x.WaitingAreaName} - {x.Name}")], [.. Platforms.Select(x => x.Name)], selectedTrainStop);
            if (newTrainStop != null)
            {
                TrainStops[TrainStops.IndexOf(selectedTrainStop)] = newTrainStop;
            }
        }
    }
    [RelayCommand]
    public async Task ImportFromInternet()
    {
        if (!CheckCanImport()) return;
        var stops = await _networkService.GetTrainNumbersAsync(SelectedStation.Name);
        if (stops.Count != 0)
        {
            TrainStops.Clear();
            foreach (TrainStop t in stops)
            {
                TrainStops.Add(RandomTrainStopProperties(t));
            }
        }
        else
        {
            _dialogService.ShowMessage("获取失败", "服务器繁忙或车站不存在。");
        }
    }
    private bool CheckCanImport()
    {
        if (TicketChecks.Count == 0)
        {
            _dialogService.ShowMessage("导入失败", "请先添加检票口。");
            return false;
        }
        if (Platforms.Count == 0)
        {
            _dialogService.ShowMessage("导入失败", "请先添加站台。");
            return false;
        }
        if (TrainStops.Count != 0)
        {
            if (!_dialogService.GetConfirm("当前操作会清空时刻表。是否继续？"))
            {
                return false;
            }
        }
        return true;
    }
    private TrainStop RandomTrainStopProperties(TrainStop t)
    {
        t.Landmark = new[] { "红色", "绿色", "褐色", "蓝色", "紫色", "黄色", "橙色", null }[new Random().Next(8)];
        t.Length = t.Number.StartsWith('G') || t.Number.StartsWith('D') || t.Number.StartsWith('C') ? Math.Abs(t.Number.GetHashCode()) % 3 == 0 ? 8 : 16 : 18;
        t.Platform = Platforms[new Random().Next(Platforms.Count)].Name;
        t.TicketChecks = [.. TicketChecks.Where(x => x.Name == t.Platform + "A" || x.Name == t.Platform + "B").Select(x => x.Name)];
        if (t.TicketChecks.Count == 0) t.TicketChecks = [TicketChecks[new Random().Next(TicketChecks.Count)].Name];
        t.WaitingArea = TicketChecks.Where(x => x.Name == t.TicketChecks[0]).ToList()[new Random().Next(TicketChecks.Where(x => x.Name == t.TicketChecks[0]).ToList().Count)].WaitingAreaName;
        return t;
    }
    #endregion

    [RelayCommand]
    public async Task SaveChanges()
    {
        if (!(await Validate())) return;
        _databaseService.UpdateStation(SelectedStation.Name, GenerateStation(SelectedStation.Name,WaitingAreaNames,TicketChecks,TrainStops,Platforms));
        await _databaseService.SaveData();
    }

    private Task<bool> Validate()
    {
        string message = "";
        foreach(var t in TicketChecks)
        {
            if (!WaitingAreaNames.Contains(t.WaitingAreaName))
            {
                message += $"\n检票口 {t.Name} 所在的候车室 {t.WaitingAreaName} 不存在；";
            }
        }
        foreach(var s in TrainStops)
        {
            if(!Platforms.Any(x=> x.Name == s.Platform))
            {
                message += $"\n车次 {s.Number} 所分配的站台 {s.Platform} 不存在；";
            }
            if (!WaitingAreaNames.Contains(s.WaitingArea))
            {
                message += $"\n车次 {s.Number} 所分配的候车区 {s.WaitingArea} 不存在；";
            }
            else
            {
                foreach (var t in s.TicketChecks)
                {
                    if (!TicketChecks.Any(x => x.Name == t && x.WaitingAreaName == s.WaitingArea))
                    {
                        message += $"\n车次 {s.Number} 所分配的检票口 {t} 在候车区 {s.WaitingArea} 中不存在；";
                    }
                }
            }
            if (s.DepartureTime == null && s.ArrivalTime == null)
            {
                message += $"\n车次 {s.Number} 未配置到发时间；";
            }
            if (s.Length==0)
            {
                message += $"\n车次 {s.Number} 长度为0；";
            }
        }
        if (message == "") return Task.FromResult(true);
        _dialogService.ShowMessage("保存失败", $"发现 {message.Split("\n").Length-1} 个错误：{message}\n请修复所有错误后再次尝试保存。");
        return Task.FromResult(false);
    }

}