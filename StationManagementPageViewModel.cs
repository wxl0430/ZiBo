﻿using System.Text.RegularExpressions;

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

    public ObservableCollection<WaitingArea> WaitingAreas { get; private set; } = [];

    public ObservableCollection<Platform> Platforms { get; private set; } = [];

    [ObservableProperty]
    public partial List<int> PlatformOccupancy { get; set; } = [];

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
            Detail = WaitingAreas.Count.ToString()
        });
        InfoItems.Add(new InfoItem()
        {
            IconGlyph = "\uF161",
            Title = "检票口数",
            Detail = WaitingAreas.SelectMany(x=>x.TicketChecks).Count().ToString()
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
        WaitingAreas.Clear();
        Platforms.Clear();
        TrainStops.Clear();
        if (args is SelectionChangedEventArgs selectedStation && selectedStation.AddedItems.Count > 0)
        {
            SelectedStation = _databaseService.GetStationByName(selectedStation.AddedItems[0].ToString());
            foreach (WaitingArea waitingArea in SelectedStation.WaitingAreas)
            {
                WaitingAreas.Add(waitingArea);
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
        string? station = await _dialogService.GetInputAsync("请输入车站名称",string.Empty);
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
    public static Station GenerateStation(string stationName, ObservableCollection<WaitingArea> waitingAreas, ObservableCollection<TrainStop> trainStops, ObservableCollection<Platform> platforms)
    {
        var station = new Station
        {
            Name = stationName,
            WaitingAreas = [.. waitingAreas],
            TrainStops = [.. trainStops],
            Platforms = [.. platforms]
        };
        return station;
    }
    [RelayCommand]
    public async Task ImportFrom7D()
    {
        var path = _dialogService.GetFile([".exe"]);
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

    #region WaitingArea
    [RelayCommand]
    public async Task AddWaitingArea()
    {
        string? waitingAreaName = await _dialogService.GetInputAsync("请输入候车室名称",string.Empty);
        if (waitingAreaName != null)
        {
            if (WaitingAreas.Any(x=>x.Name==waitingAreaName))
            {
                await _dialogService.ShowMessageAsync("添加失败", $"名称 {waitingAreaName} 候车室已存在。");
                return;
            }
            WaitingAreas.Add(new WaitingArea { Name = waitingAreaName });
        }
    }
    [RelayCommand]
    public void DeleteWaitingArea(object selectedWaitingArea)
    {
        if (selectedWaitingArea is WaitingArea n)
        {
            WaitingAreas.Remove(n);
        }
    }
    [RelayCommand]
    public async Task DeleteAllTicketChecks()
    {
        if (!await _dialogService.GetConfirmAsync("当前操作会删除全部候车室和检票口，是否继续？")) return;
        WaitingAreas.Clear();
    }

    [RelayCommand]
    public async Task AddTicketCheck(object selectedWaitingArea)
    {
        if(selectedWaitingArea is WaitingArea waitingArea)
        {
            var input = await _dialogService.GetInputAsync("请输入检票口", "试试“1-3”/“1A-5B”");
            if (input == null) return;
            var ticketCheckNames = GenerateTicketChecks(input);
            var duplicatedNames = waitingArea.TicketChecks
                                            .Where(x => ticketCheckNames.Contains(x.Name))
                                            .Select(x => x.Name);
            if (duplicatedNames.Any())
            {
                string duplicatedNamesStr = string.Join(", ", duplicatedNames);
                await _dialogService.ShowMessageAsync("添加失败", $"检票口 {duplicatedNamesStr} 已存在。");
                return;
            }
            foreach (var name in ticketCheckNames)
            {
                waitingArea.TicketChecks.Add(new TicketCheck { Name = name });
            }
            OnPropertyChanged(nameof(WaitingAreas));
        }
        else
        {
            await _dialogService.ShowMessageAsync("错误", "请先选择候车室");
        }
    }

    [RelayCommand]
    public void DeleteItem(object selected)
    {
        if (selected is WaitingArea waitingArea)
        {
            WaitingAreas.Remove(waitingArea);
        }
        if (selected is TicketCheck ticketCheck)
        {
            var area = WaitingAreas.FirstOrDefault(w => w.TicketChecks.Contains(ticketCheck));
            area?.TicketChecks.Remove(ticketCheck);
        }
    }

    public static List<string> GenerateTicketChecks(string input)
    {
        if (!input.Contains('-'))
        {
            return [input];
        }

        string[] parts = input.Split(['-'], 2);
        if (parts.Length != 2)
        {
            return [input];
        }

        var startMatch = MyRegex().Match(parts[0]);
        var endMatch = MyRegex().Match(parts[1]);

        if (!startMatch.Success || !endMatch.Success)
        {
            return [input];
        }

        int startNum = int.Parse(startMatch.Groups[1].Value);
        string sLetter = startMatch.Groups[2].Value;
        int endNum = int.Parse(endMatch.Groups[1].Value);
        string eLetter = endMatch.Groups[2].Value;

        bool startHasLetter = !string.IsNullOrEmpty(sLetter);
        bool endHasLetter = !string.IsNullOrEmpty(eLetter);

        if (startHasLetter != endHasLetter)
        {
            return [input];
        }

        int min = Math.Min(startNum, endNum);
        int max = Math.Max(startNum, endNum);
        List<int> numbers = [.. Enumerable.Range(min, max - min + 1)];

        List<string> result = [];

        if (startHasLetter)
        {
            if (sLetter == eLetter)
            {
                numbers.ForEach(n => result.Add($"{n}{sLetter}"));
            }
            else
            {
                numbers.ForEach(n =>
                {
                    result.Add($"{n}A");
                    result.Add($"{n}B");
                });
            }
        }
        else
        {
            numbers.ForEach(n => result.Add(n.ToString()));
        }
        return result;
    }

    [GeneratedRegex(@"^(\d+)([AB]?)$")]
    private static partial Regex MyRegex();

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
        string warningMessage = "";
        var parsedStops = new List<TrainStop>();
        foreach (TrainNumber trainNumber in trainNumbers)
        {
            var t = trainNumber.TimeTable.FirstOrDefault(x => x.Station == SelectedStation.Name);
            if (t == null) continue;
            t.Number = trainNumber.Number;
            t.Origin = trainNumber.TimeTable.First().Station;
            t.Terminal = trainNumber.TimeTable.Last().Station;
            parsedStops.Add(t);
        }
        var sortedStops = parsedStops
            .Select(stop => new 
            {
                Stop = stop,
                EffectiveArrival = stop.ArrivalTime ?? stop.DepartureTime?.Subtract(TimeSpan.FromMinutes(10)),
                EffectiveDeparture = stop.DepartureTime ?? stop.ArrivalTime?.Add(TimeSpan.FromMinutes(10))
            })
            .OrderBy(item => item.EffectiveArrival)
            .ThenBy(item => item.EffectiveDeparture)
            .Select(item => item.Stop)
            .ToList();
        TrainStops.Clear();
        PlatformOccupancy.Clear();
        for (int i = 0; i < Platforms.Count; i++)
        {
            PlatformOccupancy.Add(0);
        }
        foreach (var stop in sortedStops)
        {
            var processedStop = RandomTrainStopProperties(stop, ref warningMessage);
            TrainStops.Add(processedStop);
        }
        if (warningMessage != "") await _dialogService.ShowTextAsync("警告", $"发现 {warningMessage.Split("\n").Length - 1} 个警告：{warningMessage}");
    }
    [RelayCommand]
    public async Task ImportFromLulutong()
    {
        if (!await CheckCanImport()) return;
        var path = _dialogService.GetFile([".csv"]);
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
            var parsedStops = new List<TrainStop>();
            string warningMessage = "";
            PlatformOccupancy.Clear();
            for (int i = 0; i < Platforms.Count; i++)
            {
                PlatformOccupancy.Add(0);
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
                    
                    parsedStops.Add(trainStop);
                }
            }

            var sortedStops = parsedStops
                .Select(stop => new 
                {
                    Stop = stop,
                    EffectiveArrival = stop.ArrivalTime ?? stop.DepartureTime?.Subtract(TimeSpan.FromMinutes(10)),
                    EffectiveDeparture = stop.DepartureTime ?? stop.ArrivalTime?.Add(TimeSpan.FromMinutes(10))
                })
                .OrderBy(item => item.EffectiveArrival)
                .ThenBy(item => item.EffectiveDeparture)
                .Select(item => item.Stop)
                .ToList();

            foreach (var stop in sortedStops)
            {
                TrainStops.Add(RandomTrainStopProperties(stop, ref warningMessage));
            }
            if (warningMessage != "") await _dialogService.ShowTextAsync("警告", $"发现 {warningMessage.Split("\n").Length - 1} 个警告：{warningMessage}");
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

        var path = _dialogService.GetFile([".pyetgr",".pyetdb",".json"]);
        if (path == null) return;

        string warningMessage = "";
        var parsedStops = new List<TrainStop>();

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

                parsedStops.Add(trainStop);
            }

            var sortedStops = parsedStops
                .Select(stop => new
                {
                    Stop = stop,
                    EffectiveArrival = stop.ArrivalTime ?? stop.DepartureTime?.Subtract(TimeSpan.FromMinutes(10)),
                    EffectiveDeparture = stop.DepartureTime ?? stop.ArrivalTime?.Add(TimeSpan.FromMinutes(10))
                })
                .OrderBy(item => item.EffectiveArrival)
                .ThenBy(item => item.EffectiveDeparture)
                .Select(item => item.Stop)
                .ToList();

            PlatformOccupancy.Clear();
            for (int i = 0; i < Platforms.Count; i++)
            {
                PlatformOccupancy.Add(0);
            }

            TrainStops.Clear();
            foreach (var stop in sortedStops)
            {
                TrainStops.Add(RandomTrainStopProperties(stop, ref warningMessage));
            }

            if (warningMessage != "") await _dialogService.ShowTextAsync("警告", $"发现 {warningMessage.Split("\n").Length - 1} 个警告：{warningMessage}");
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
        var path = _dialogService.GetFile([".xlsx"]);
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
                    worksheet.Cells[row, 8].Text.Trim() == "" ||
                    worksheet.Cells[row, 9].Text.Trim() == "")
                {
                    continue;
                }

                TrainStops.Add(new TrainStop
                {
                    Number = worksheet.Cells[row, 1].Text.Trim(),
                    Length = int.TryParse(worksheet.Cells[row, 2].Text, out int length) ? length : 0,
                    ArrivalTime = TimeSpan.TryParseExact(worksheet.Cells[row, 3].Text, @"hh\:mm", null, out TimeSpan arrival) ? arrival : null,
                    DepartureTime = TimeSpan.TryParseExact(worksheet.Cells[row, 4].Text, @"hh\:mm", null, out TimeSpan departure) ? departure : null,
                    TicketCheckIds = [.. worksheet.Cells[row, 5].Text
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(entry =>
                        {
                            var parts = entry.Split('|');
                            if (parts.Length < 2)
                                return Guid.Empty; 
                            var waitingAreaName = parts[0];
                            var ticketCheckName = parts[1];
                            var waitingArea = WaitingAreas.FirstOrDefault(w => w.Name == waitingAreaName);
                            TicketCheck tc;
                            if (waitingArea == null)
                            {
                                tc = new TicketCheck { Name = ticketCheckName };
                                WaitingAreas.Add(new WaitingArea
                                {
                                    Name = waitingAreaName,
                                    TicketChecks = [tc]
                                });
                            }
                            else
                            {
                                tc = waitingArea.TicketChecks.FirstOrDefault(c => c.Name == ticketCheckName);
                                if (tc == null)
                                {
                                    tc = new TicketCheck { Name = ticketCheckName };
                                    waitingArea.TicketChecks.Add(tc);
                                }
                            }
                            return tc.Id;
                        })
                        .Where(id => id != Guid.Empty)],
                    Platform = worksheet.Cells[row, 6].Text.Trim(),
                    Landmark = worksheet.Cells[row, 7].Text.Trim() == "" ? "无" : worksheet.Cells[row, 8].Text.Trim(),
                    Origin = worksheet.Cells[row, 8].Text.Trim(),
                    Terminal = worksheet.Cells[row, 9].Text.Trim()
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
        var path = _dialogService.SaveFile(".xlsx", "data");
        if (path == null) return;
        ExcelPackage.License.SetNonCommercialPersonal("CRSim");
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        worksheet.Cells[1, 1].Value = "车次";
        worksheet.Cells[1, 2].Value = "长度";
        worksheet.Cells[1, 3].Value = "到时";
        worksheet.Cells[1, 4].Value = "发时";
        worksheet.Cells[1, 5].Value = "检票口";
        worksheet.Cells[1, 6].Value = "站台";
        worksheet.Cells[1, 7].Value = "地标";
        worksheet.Cells[1, 8].Value = "始发站";
        worksheet.Cells[1, 9].Value = "终到站";
        for (int i = 0; i < TrainStops.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = TrainStops[i].Number;
            worksheet.Cells[i + 2, 2].Value = TrainStops[i].Length;
            worksheet.Cells[i + 2, 3].Value = TrainStops[i].ArrivalTime?.ToString(@"hh\:mm") ?? string.Empty;
            worksheet.Cells[i + 2, 4].Value = TrainStops[i].DepartureTime?.ToString(@"hh\:mm") ?? string.Empty;
            worksheet.Cells[i + 2, 5].Value = TrainStops[i].TicketCheckIds is null ? string.Empty : string.Join(" ", TrainStops[i].TicketCheckIds
                .Select(id =>
                {
                    var area = WaitingAreas.FirstOrDefault(a => a.TicketChecks.Any(tc => tc.Id == id));
                    var check = area?.TicketChecks.FirstOrDefault(tc => tc.Id == id);
                    if (area != null && check != null)
                        return $"{area.Name}|{check.Name}";
                    else
                        return null;
                })
                .Where(x => x != null));
            worksheet.Cells[i + 2, 6].Value = TrainStops[i].Platform;
            worksheet.Cells[i + 2, 7].Value = TrainStops[i].Landmark;
            worksheet.Cells[i + 2, 8].Value = TrainStops[i].Origin;
            worksheet.Cells[i + 2, 9].Value = TrainStops[i].Terminal;
        }
        await package.SaveAsAsync(new FileInfo(path));
    }
    [RelayCommand]
    public async Task AddTrainStop()
    {
        var newTrainStop = await _dialogService.GetInputTrainStopAsync([.. WaitingAreas], [.. Platforms.Select(x => x.Name)]);
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
            var newTrainStop = await _dialogService.EditInputTrainStopAsync([.. WaitingAreas], [.. Platforms.Select(x => x.Name)], selectedTrainStop);
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
        var stops = (await _networkService.GetTrainNumbersAsync(SelectedStation.Name))
            .Select(stop => new 
            {
                Stop = stop,
                EffectiveArrival = stop.ArrivalTime ?? stop.DepartureTime?.Subtract(TimeSpan.FromMinutes(10)),
                EffectiveDeparture = stop.DepartureTime ?? stop.ArrivalTime?.Add(TimeSpan.FromMinutes(10))
            })
            .OrderBy(item => item.EffectiveArrival)
            .ThenBy(item => item.EffectiveDeparture)
            .Select(item => item.Stop)
            .ToList();
        string warningMessage = "";
        PlatformOccupancy.Clear();
        for (int i = 0; i < Platforms.Count; i++)
        {
            PlatformOccupancy.Add(0);
        }
        if (stops is not null && stops.Count != 0)
        {
            TrainStops.Clear();
            foreach (TrainStop t in stops)
            {
                TrainStops.Add(RandomTrainStopProperties(t, ref warningMessage));
            }
            if (warningMessage != "") await _dialogService.ShowTextAsync("警告", $"发现 {warningMessage.Split("\n").Length - 1} 个警告：{warningMessage}");
        }
        else
        {
            await _dialogService.ShowMessageAsync("获取失败", "服务器繁忙或车站不存在。");
        }
    }
    private async Task<bool> CheckCanImport()
    {
        if (WaitingAreas.All(x => x.TicketChecks.Count == 0))
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
    private TrainStop RandomTrainStopProperties(TrainStop t, ref string warningMessage)
    {
        t.Landmark = new[] { "红色", "绿色", "褐色", "蓝色", "紫色", "黄色", "橙色", null }[new Random().Next(8)];
        t.Length = t.Number.StartsWith('G') || t.Number.StartsWith('D') || t.Number.StartsWith('C') ? Math.Abs(t.Number.GetHashCode()) % 3 == 0 ? 8 : 16 : 18;
        // t.Platform = Platforms[new Random().Next(Platforms.Count)].Name; //随机分配站台

        //随机分配无冲突站台
        List<int> CanUse = new List<int>();
        double? arrivalMinutes = t.ArrivalTime?.TotalMinutes;
        double? departureMinutes = t.DepartureTime?.TotalMinutes;
        int ArrivalTime = (int)(arrivalMinutes ?? (departureMinutes - 10));
        int DepartureTime = (int)(departureMinutes ?? (arrivalMinutes + 10));
        for (int i = 0; i < PlatformOccupancy.Count; i++)
        {
            if (PlatformOccupancy[i] < ArrivalTime)
            {
                CanUse.Add(i);
            }
        }
        if (CanUse.Count() > 0){
            int id = CanUse[new Random().Next(CanUse.Count)];
            PlatformOccupancy[id] = DepartureTime;
            t.Platform = Platforms[id].Name;
        }else{
            int id=new Random().Next(Platforms.Count);
            PlatformOccupancy[id] = DepartureTime;
            warningMessage += $"车次 {t.Number} 无可用站台，强制随机分配到{Platforms[id].Name}站台\n";
            t.Platform = Platforms[id].Name;
        }
        
        if (t.DepartureTime.HasValue)
        {
            //t.TicketCheckIds = [.. WaitingAreas
            //    .FirstOrDefault(x => x.TicketChecks.Any(x => x.Name == t.Platform + "A")
            //             && x.TicketChecks.Any(x => x.Name == t.Platform + "B"))?
            //    .TicketChecks
            //    .Where(x => x.Name == t.Platform + "A" || x.Name == t.Platform + "B")
            //    .Select(x => x.Id)];
            //根据站台选择检票口
            if (t.TicketCheckIds is null || t.TicketCheckIds.Count == 0) t.TicketCheckIds = [WaitingAreas
                .SelectMany(x => x.TicketChecks)
                .Select(x => x.Id)
                .OrderBy(_ => new Random().Next())
                .FirstOrDefault()];
            //若没有则随机匹配
        }
        return t;
    }
    #endregion

    [RelayCommand]
    public async Task SaveChanges()
    {
        if (!await Validate()) return;
        _databaseService.UpdateStation(SelectedStation.Name, GenerateStation(SelectedStation.Name, WaitingAreas, TrainStops, Platforms));
        await _databaseService.SaveData();
        UpdateInfoItems();
    }

    private async Task<bool> Validate()
    {
        string warningMessage = "";
        string errorMessage = "";
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

        foreach (var s in TrainStops)
        {
            if (s.DepartureTime.HasValue)
            {
                List<Guid> allTicketChecks = [.. WaitingAreas.SelectMany(x => x.TicketChecks.Select(x=>x.Id))];
                foreach (var ticketCheckId in s.TicketCheckIds)
                {
                    if (!allTicketChecks.Contains(ticketCheckId))
                    {
                        warningMessage += $"\n车次 {s.Number} 所分配的检票口 {ticketCheckId} 不存在，已自动删除；";
                        s.TicketCheckIds.Remove(ticketCheckId);//感觉这个代码放在这个位置不合适，但我也不知放在哪里好QAQ
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