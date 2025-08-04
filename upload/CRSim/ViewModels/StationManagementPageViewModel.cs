using CRSim.Core.Abstractions;

namespace CRSim.ViewModels;

public partial class StationManagementPageViewModel : ObservableObject
{
    public ObservableCollection<InfoItem> InfoItems { get; set; } = [];

    [ObservableProperty]
    public partial string PageTitle { get; set; } = "车站管理";

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = false;

    [ObservableProperty]
    public partial List<string> StationNames { get; set; } = [];

    [ObservableProperty]
    public partial Station SelectedStation { get; set; } = new();

    public ObservableCollection<string> WaitingAreaNames { get; private set; } = [];
    public ObservableCollection<Platform> Platforms { get; private set; } = [];

    public ObservableCollection<TicketCheck> TicketChecks { get; private set; } = [];

    public ObservableCollection<TrainStop> TrainStops { get; private set; } = [];

    private readonly IDatabaseService _databaseService = App.AppHost.Services.GetService<IDatabaseService>();

    private readonly IDialogService _dialogService = App.AppHost.Services.GetService<IDialogService>();

    private readonly INetworkService _networkService = App.AppHost.Services.GetService<INetworkService>();
    public StationManagementPageViewModel()
    {
        RefreshStations();
    }
    private void UpdateInfoItems()
    {
        InfoItems.Clear();
        InfoItems.Add(new InfoItem()
        {
            IconGlyph = "\uEC08",
            Title = "车站名称",
            Detail = SelectedStation.Name
        });
        InfoItems.Add(new InfoItem()
        {
            IconGlyph = "\uE7C0",
            Title = "列车停靠数",
            Detail = TrainStops.Count.ToString()
        });
        InfoItems.Add(new InfoItem()
        {
            IconGlyph = "\uECA5",
            Title = "候车室数",
            Detail = WaitingAreaNames.Count.ToString()
        });
        InfoItems.Add(new InfoItem()
        {
            IconGlyph = "\uF161",
            Title = "检票口数",
            Detail = TicketChecks.Count.ToString()
        });
        InfoItems.Add(new InfoItem()
        {
            IconGlyph = "\uE799",
            Title = "站台数",
            Detail = Platforms.Count.ToString()
        });
    }

#region Station
    public void RefreshStations()
    {
        var stationsList = _databaseService.GetAllStations();
        StationNames = [.. stationsList.Select(s => s.Name)];
    }
    [RelayCommand]
    public void StationSelected(object args)
    {
        WaitingAreaNames.Clear();
        Platforms.Clear();
        TicketChecks.Clear();
        TrainStops.Clear();
        if (args is SelectionChangedEventArgs selectedStation && selectedStation.AddedItems.Count > 0)
        {
            SelectedStation = _databaseService.GetStationByName(selectedStation.AddedItems[0].ToString());
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
            UpdateInfoItems();
        }
        else
        {
            IsSelected = false;
        }
    }

    [RelayCommand]
    public async Task AddStation()
    {
        string? station = await _dialogService.GetInputAsync("请输入车站名称");
        if (station != null)
        {
            if (StationNames.Contains(station))
            {
                await _dialogService.ShowMessageAsync("添加失败", $"车站 {station} 已存在。");
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
        if (!await _dialogService.GetConfirmAsync("当前操作会删除全部车站，是否继续？")) return;
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
        var path = await _dialogService.GetFileAsync([".exe"]);
        if (path == null) return;
        try
        {
            await _databaseService.ImportStationFrom7D(path);
            await _databaseService.SaveData();
            RefreshStations();
        }
        catch (Exception e)
        {
            await _dialogService.ShowMessageAsync("导入失败", "文件格式错误或被占用。\n" + e.Message);
        }
    }
    #endregion

    #region TicketCheck
    [RelayCommand]
    public async Task AddTicketCheck()
    {
        (string waitingAreaName, List<string> ticketChecks) = await _dialogService.GetInputTicketCheckAsync([.. WaitingAreaNames]);
        if (ticketChecks != null && waitingAreaName != null)
        {
            foreach (string ticketCheck in ticketChecks)
            {
                if (TicketChecks.Where(x => x.WaitingAreaName == waitingAreaName).Select(x => x.Name).Contains(ticketCheck))
                {
                    await _dialogService.ShowMessageAsync("添加失败", $"编号 {ticketCheck} 检票口已存在。");
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
    public async Task DeleteAllTicketCheck()
    {
        if (!await _dialogService.GetConfirmAsync("当前操作会删除全部检票口，是否继续？")) return;
        TicketChecks.Clear();
    }
    #endregion

    #region WaitingArea
    [RelayCommand]
    public async Task AddWaitingArea()
    {
        string? waitingAreaName = await _dialogService.GetInputAsync("请输入候车室名称");
        if (waitingAreaName != null)
        {
            if (WaitingAreaNames.Contains(waitingAreaName))
            {
                await _dialogService.ShowMessageAsync("添加失败", $"名称 {waitingAreaName} 候车室已存在。");
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
    public async Task DeleteAllWaitingArea()
    {
        if (!await _dialogService.GetConfirmAsync("当前操作会删除全部候车室，是否继续？")) return;
        WaitingAreaNames.Clear();
    }
    #endregion

    #region Platform
    [RelayCommand]
    public async Task AddPlatform()
    {
        var platforms = await _dialogService.GetInputPlatformAsync();
        if (platforms != null)
        {
            foreach (var platform in platforms)
            {
                if (Platforms.Any(x => x.Name == platform.Name))
                {
                    await _dialogService.ShowMessageAsync("添加失败", $"编号 {platform.Name} 站台已存在。");
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
    public async Task DeleteAllPlatform()
    {
        if (!await _dialogService.GetConfirmAsync("当前操作会删除全部站台，是否继续？")) return;
        Platforms.Clear();
    }
    #endregion

    #region TrainStop
    [RelayCommand]
    public async Task ImportFromTrainNumbers()
    {
        if (!await CheckCanImport()) return;
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
        if (!await CheckCanImport()) return;
        var path = await _dialogService.GetFileAsync([".csv"]);
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
        catch (Exception e)
        {
            await _dialogService.ShowMessageAsync("导入失败", "文件格式错误或被占用。\n" + e.Message);
        }
    }
    [RelayCommand]
    public async Task ImportFrompyETRC()
    {
        if (!await CheckCanImport()) return;

        var path = await _dialogService.GetFileAsync([".pyetgr",".pyetdb",".json"]);
        if (path == null) return;
        try
        {
            using var fs = File.OpenRead(path);
            var doc = await JsonDocument.ParseAsync(fs);

            var root = doc.RootElement;
            if (!root.TryGetProperty("trains", out var trains)) return;

            foreach (var train in trains.EnumerateArray())
            {
                if (!train.TryGetProperty("timetable", out var timetable)) continue;
                var stops = timetable.EnumerateArray().ToArray();
                if (stops.Length == 0) continue;

                string origin = stops[0].GetProperty("zhanming").GetString();
                string terminal = stops[^1].GetProperty("zhanming").GetString();

                TimeSpan? arrivalTime = null;
                TimeSpan? departureTime = null;

                foreach (var stop in stops)
                {
                    string name = stop.GetProperty("zhanming").GetString();
                    if (name != SelectedStation.Name) continue;

                    string cfsj = stop.GetProperty("cfsj").GetString();
                    string ddsj = stop.GetProperty("ddsj").GetString();

                    if (cfsj == ddsj)
                    {
                        if (name == origin)
                        {
                            departureTime = RoundToMinute(cfsj);
                            arrivalTime = null;
                        }
                        else if (name == terminal)
                        {
                            departureTime = null;
                            arrivalTime = RoundToMinute(ddsj);
                        }
                    }
                    else
                    {
                        departureTime = RoundToMinute(cfsj);
                        arrivalTime = RoundToMinute(ddsj);
                    }
                    break;
                }
                if (arrivalTime == null && departureTime == null) continue;
                string number = train.GetProperty("checi").EnumerateArray().FirstOrDefault().GetString()?.Split('/')?.FirstOrDefault() ?? string.Empty;

                var trainStop = new TrainStop
                {
                    Number = number,
                    Origin = origin,
                    Terminal = terminal,
                    ArrivalTime = arrivalTime,
                    DepartureTime = departureTime,
                };

                TrainStops.Add(RandomTrainStopProperties(trainStop));
            }
        }
        catch (Exception e)
        {
            await _dialogService.ShowMessageAsync("导入失败", "文件格式错误或被占用。\n" + e.Message);
        }
    }
    private static TimeSpan RoundToMinute(string timeString)
    {
        if (TimeSpan.TryParse(timeString, out var time))
            return TimeSpan.FromMinutes(Math.Round(time.TotalMinutes));
        return default;
    }
    [RelayCommand]
    public async Task ImportFromExcel()
    {
        if (TrainStops.Count != 0)
        {
            if (!await _dialogService.GetConfirmAsync("当前操作可能会清空时刻表。是否继续？"))
            {
                return;
            }
        }
        var path = await _dialogService.GetFileAsync([".xlsx"]);
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
        catch (Exception e)
        {
            await _dialogService.ShowMessageAsync("导入失败", "文件格式错误或被占用。\n" + e.Message);
        }
    }
    [RelayCommand]
    public async Task ExportToExcel()
    {
        var path = await _dialogService.SaveFileAsync(".xlsx", "data");
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
        await package.SaveAsAsync(new FileInfo(path));
    }
    [RelayCommand]
    public async Task AddTrainStop()
    {
        var newTrainStop = await _dialogService.GetInputTrainStopAsync([.. TicketChecks.Select(x => $"{x.WaitingAreaName} - {x.Name}")], [.. Platforms.Select(x => x.Name)]);
        if (newTrainStop != null)
        {
            if (TrainStops.Any(x => x.Number == newTrainStop.Number))
            {
                await _dialogService.ShowMessageAsync("添加失败", $"车次 {newTrainStop.Number} 已存在。");
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
    public async Task DeleteAllTrainStop()
    {
        if (!await _dialogService.GetConfirmAsync("当前操作会删除全部车次，是否继续？")) return;
        TrainStops.Clear();
    }
    [RelayCommand]
    public async Task EditTrainStop(object _selectedTrainStop)
    {
        if (_selectedTrainStop is TrainStop selectedTrainStop)
        {
            var newTrainStop = await _dialogService.EditInputTrainStopAsync([.. TicketChecks.Select(x => $"{x.WaitingAreaName} - {x.Name}")], [.. Platforms.Select(x => x.Name)], selectedTrainStop);
            if (newTrainStop != null)
            {
                if (TrainStops.Where(x => x.Number == newTrainStop.Number).Count() > 1)
                {
                    await _dialogService.ShowMessageAsync("修改失败", $"车次 {newTrainStop.Number} 已存在。");
                    return;
                }
                TrainStops[TrainStops.IndexOf(selectedTrainStop)] = newTrainStop;
            }
        }
    }
    [RelayCommand]
    public async Task ImportFromInternet()
    {
        if (!await CheckCanImport()) return;
        var stops = await _networkService.GetTrainNumbersAsync(SelectedStation.Name);
        if (stops is not null && stops.Count != 0)
        {
            TrainStops.Clear();
            foreach (TrainStop t in stops)
            {
                TrainStops.Add(RandomTrainStopProperties(t));
            }
        }
        else
        {
            await _dialogService.ShowMessageAsync("获取失败", "服务器繁忙或车站不存在。");
        }
    }
    private async Task<bool> CheckCanImport()
    {
        if (TicketChecks.Count == 0)
        {
            await _dialogService.ShowMessageAsync("导入失败", "请先添加检票口。");
            return false;
        }
        if (Platforms.Count == 0)
        {
            await _dialogService.ShowMessageAsync("导入失败", "请先添加站台。");
            return false;
        }
        if (TrainStops.Count != 0)
        {
            if (!await _dialogService.GetConfirmAsync("当前操作会清空时刻表。是否继续？"))
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
        if (t.DepartureTime.HasValue)
        {
            t.TicketChecks = [.. TicketChecks.Where(x => x.Name == t.Platform + "A" || x.Name == t.Platform + "B").Select(x => x.Name)];
            if (t.TicketChecks.Count == 0) t.TicketChecks = [TicketChecks[new Random().Next(TicketChecks.Count)].Name];
            t.WaitingArea = TicketChecks.Where(x => x.Name == t.TicketChecks[0]).ToList()[new Random().Next(TicketChecks.Where(x => x.Name == t.TicketChecks[0]).ToList().Count)].WaitingAreaName;
        }
        return t;
    }
    #endregion

    [RelayCommand]
    public async Task SaveChanges()
    {
        if (!await Validate()) return;
        _databaseService.UpdateStation(SelectedStation.Name, GenerateStation(SelectedStation.Name, WaitingAreaNames, TicketChecks, TrainStops, Platforms));
        await _databaseService.SaveData();
        UpdateInfoItems();
    }

    private async Task<bool> Validate()
    {
        string warningMessage = "";
        var platformGroups = TrainStops
            .Where(ts => !string.IsNullOrEmpty(ts.Platform) && ts.DepartureTime != null && ts.ArrivalTime != null)
            .GroupBy(ts => ts.Platform);
        foreach (var group in platformGroups)
        {
            var stops = group.OrderBy(ts => ts.ArrivalTime ?? TimeSpan.Zero).ToList();
            for (int i = 0; i < stops.Count - 1; i++)
            {
                var current = stops[i];
                var next = stops[i + 1];
                // 判断时间是否重叠
                if (current.DepartureTime != null && next.ArrivalTime != null &&
                    current.DepartureTime > next.ArrivalTime)
                {
                    warningMessage += $"\n站台 {group.Key} 车次 {current.Number} ({current.ArrivalTime:hh\\:mm}-{current.DepartureTime:hh\\:mm}) 与车次 {next.Number} ({next.ArrivalTime:hh\\:mm}-{next.DepartureTime:hh\\:mm}) 时间重叠；";
                }
            }
        }

        string errorMessage = "";
        foreach (var t in TicketChecks)
        {
            if (!WaitingAreaNames.Contains(t.WaitingAreaName))
            {
                errorMessage += $"\n检票口 {t.Name} 所在的候车室 {t.WaitingAreaName} 不存在；";
            }
        }
        foreach (var s in TrainStops)
        {
            if (s.DepartureTime.HasValue)
            {
                if (!WaitingAreaNames.Contains(s.WaitingArea))
                {
                    errorMessage += $"\n车次 {s.Number} 所分配的候车区 {s.WaitingArea} 不存在；";
                }
                else
                {
                    foreach (var t in s.TicketChecks)
                    {
                        if (!TicketChecks.Any(x => x.Name == t && x.WaitingAreaName == s.WaitingArea))
                        {
                            errorMessage += $"\n车次 {s.Number} 所分配的检票口 {t} 在候车区 {s.WaitingArea} 中不存在；";
                        }
                    }
                }
            }
            if (!Platforms.Any(x => x.Name == s.Platform))
            {
                errorMessage += $"\n车次 {s.Number} 所分配的站台 {s.Platform} 不存在；";
            }
            if (s.DepartureTime == null && s.ArrivalTime == null)
            {
                errorMessage += $"\n车次 {s.Number} 未配置到发时间；";
            }
            if (s.Length == 0)
            {
                errorMessage += $"\n车次 {s.Number} 长度为0；";
            }
        }
        if (errorMessage == "")
        {
            if (warningMessage != "") await _dialogService.ShowTextAsync("警告", $"发现 {warningMessage.Split("\n").Length - 1} 个警告：{warningMessage}\n配置已保存，您可以选择忽略这些警告，亦或手动优化。");
            return true;
        }
        await _dialogService.ShowTextAsync("保存失败", $"发现 {errorMessage.Split("\n").Length - 1} 个错误和 {warningMessage.Split("\n").Length - 1} 个警告：{errorMessage}{warningMessage}\n请修复所有错误后再次尝试保存。");
        return false;
    }

}