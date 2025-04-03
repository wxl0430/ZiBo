using CRSim.Core.Models;
using OfficeOpenXml;
using System;
using System.Collections.Specialized;
using System.IO;

namespace CRSim.ViewModels;

public partial class TrainNumberManagementPageViewModel : ObservableObject 
{
	[ObservableProperty]
	private string _pageTitle = "车次管理";

	[ObservableProperty]
	private string _pageDescription = "看起来不是很温馨的提醒：改完记得保存! Owo";

    [ObservableProperty]
    private bool _isSelected = false;

    public ObservableCollection<TrainNumber> TrainNumbers { get; set; } = [];

    [ObservableProperty]
    private TrainNumber _selectedTrainNumber = new();

    [ObservableProperty]
    private int _progressValue = 0;

    public ObservableCollection<TrainStop> TimeTable { get; private set; }= [];

    public ObservableCollection<Section> Sections { get; private set; } = [];

    private readonly IDatabaseService _databaseService;

    private readonly IDialogService _dialogService;

    private readonly INetworkService _networkService;

    #region CheckBox Status
    [ObservableProperty]
    private bool _sWChecked = false;
    [ObservableProperty]
    private bool _zYYChecked = false;
    [ObservableProperty]
    private bool _zYChecked = false;
    [ObservableProperty]
    private bool _zEChecked = false;
    [ObservableProperty]
    private bool _rWChecked = false;
    [ObservableProperty]
    private bool _yWChecked = false;
    [ObservableProperty]
    private bool _gRWChecked = false;
    [ObservableProperty]
    private bool _rZChecked = false;
    [ObservableProperty]
    private bool _yZChecked = false;
    [ObservableProperty]
    private bool _wZChecked = false;
    #endregion

    public TrainNumberManagementPageViewModel(IDatabaseService databaseService,IDialogService dialogService,INetworkService networkService)
    {
		_databaseService = databaseService;
        _dialogService = dialogService;
        _networkService = networkService;
        RefreshTrainNumbers();
    }

    #region TrainNumber
    public void RefreshTrainNumbers()
    {
        TrainNumbers.Clear();
        foreach (var t in _databaseService.GetAllTrainNumbers())
        {
            TrainNumbers.Add(t);
        }
    }

    [RelayCommand]
    public void TrainNumberSelected(object selectedNumber)
    {
        if (selectedNumber is TrainNumber s)
        {
            TimeTable.Clear();
            Sections.Clear();
            SelectedTrainNumber = _databaseService.GetTrainNumberByNumber(s.Number);
            foreach (TrainStop t in SelectedTrainNumber.TimeTable)
            {
                TimeTable.Add(t);
            }
            foreach (Section section in SelectedTrainNumber.Sections)
            {
                Sections.Add(section);
            }
            if (Sections.Count != 0)
            {
                SWChecked = Sections[0].Tickets.SW != null;
                ZYYChecked = Sections[0].Tickets.ZYY != null;
                ZYChecked = Sections[0].Tickets.ZY != null;
                ZEChecked = Sections[0].Tickets.ZE != null;
                GRWChecked = Sections[0].Tickets.GRW != null;
                RWChecked = Sections[0].Tickets.RW != null;
                YWChecked = Sections[0].Tickets.YW != null;
                RZChecked = Sections[0].Tickets.RZ != null;
                YZChecked = Sections[0].Tickets.YZ != null;
                WZChecked = Sections[0].Tickets.WZ != null;
            }
            else
            {
                SWChecked = false;
                ZYYChecked = false;
                ZYChecked = false;   
                ZEChecked = false;
                GRWChecked = false;
                RWChecked = false;
                YWChecked = false;
                RZChecked = false;
                YZChecked = false;
                WZChecked = false;
            }
            IsSelected = true;
        }
        if(selectedNumber == null)
        {
            IsSelected = false;
        }
    }
    [RelayCommand]
    public async Task AddTrainNumber()
    {
        string? number = _dialogService.GetInput("请输入车次号")?.ToUpper();
        if (number != null)
        {
            if (TrainNumbers.Select(x => x.Number).Contains(number))
            {
                _dialogService.ShowMessage("添加失败", $"车次号 {number} 已存在。");
                return;
            }
            _databaseService.AddTrainNumber(number);
            await _databaseService.SaveData();
            RefreshTrainNumbers();
        }
    }
    [RelayCommand]
    public async Task DeleteTrainNumber(object selectedTrainNumber)
    {
        if (selectedTrainNumber is TrainNumber t)
        {
            _databaseService.DeleteTrainNumber(t);
            await _databaseService.SaveData();
            RefreshTrainNumbers();
        }
    }
    [RelayCommand]
    public async Task DeleteAllTrainNumbers()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部车次，是否继续？")) return;
        var trainNumbers = _databaseService.GetAllTrainNumbers().ToList();
        foreach (TrainNumber trainNumber in trainNumbers)
        {
            _databaseService.DeleteTrainNumber(trainNumber);
        }
        await _databaseService.SaveData();
        RefreshTrainNumbers();
    }
    [RelayCommand]
    public async Task ImportFromLulutong()
    {
        var path = _dialogService.GetFile("CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*");
        if (path == null) return;
        try
        {
            var numbers = ReadCsvFirstColumn(path);
            var intersection = numbers.Intersect(TrainNumbers.Select(x => x.Number)).ToList();
            if (intersection.Count != 0)
            {
                if (!_dialogService.GetConfirm("当前操作可能覆盖现有车次配置，是否继续？")) return;
            }
            foreach (string s in intersection)
            {
                _databaseService.DeleteTrainNumber(_databaseService.GetTrainNumberByNumber(s));
            }
            RefreshTrainNumbers();
            int total = numbers.Count;
            int current = 0;
            foreach (string number in numbers)
            {
                current++;
                ProgressValue = (int)((double)current / total * 100);
                List<TrainStop>? timeTable = await _networkService.GetTimeTableAsync(number);
                await Task.Delay(100);
                timeTable ??= [];
                _databaseService.AddTrainNumber(number);
                var sections = new List<Section>();
                var isEmu = number.StartsWith('G') || number.StartsWith('D') || number.StartsWith('C');
                for (int i = 1; i < timeTable.Count; i++)
                {
                    sections.Add(new Section
                    {
                        From = timeTable[i - 1].Station,
                        To = timeTable[i].Station,
                        Tickets = new Tickets
                        {
                            SW = isEmu ? new() : null,
                            ZYY = isEmu ? new() : null,
                            ZY = isEmu ? new() : null,
                            ZE = isEmu ? new() : null,
                            RZ = !isEmu ? new() : null,
                            GRW = !isEmu ? new() : null,
                            RW = !isEmu ? new() : null,
                            WZ = new(),
                            YW = !isEmu ? new() : null,
                            YZ = !isEmu ? new() : null
                        }
                    });
                }
                _databaseService.UpdateTrainNumber(_databaseService.GetTrainNumberByNumber(number), timeTable, sections);
            }
            ProgressValue = 0;
            await _databaseService.SaveData();
            RefreshTrainNumbers();
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
            ProgressValue = 0;
        }
    }
    #endregion

    #region TimeTable
    [RelayCommand]
    public async Task TimeTableImportFromInternetBatch()
    {
        if (!_dialogService.GetConfirm("当前操作会覆盖现有车次配置，是否继续？")) return;
        TrainNumberSelected(null);
        var numbers = TrainNumbers.Select(x => x.Number).ToList();
        foreach (string s in numbers)
        {
            _databaseService.DeleteTrainNumber(_databaseService.GetTrainNumberByNumber(s));
        }
        RefreshTrainNumbers();
        int total = numbers.Count;
        int current = 0;
        foreach (string number in numbers)
        {
            current++;
            ProgressValue = (int)((double)current / total * 100);
            List<TrainStop>? timeTable = await _networkService.GetTimeTableAsync(number);
            await Task.Delay(100);
            timeTable ??= [];
            _databaseService.AddTrainNumber(number);
            var sections = new List<Section>();
            var isEmu = number.StartsWith('G') || number.StartsWith('D') || number.StartsWith('C');
            for (int i = 1; i < timeTable.Count; i++)
            {
                sections.Add(new Section
                {
                    From = timeTable[i - 1].Station,
                    To = timeTable[i].Station,
                    Tickets = new Tickets
                    {
                        SW = isEmu ? new() : null,
                        ZYY = isEmu ? new() : null,
                        ZY = isEmu ? new() : null,
                        ZE = isEmu ? new() : null,
                        RZ = !isEmu ? new() : null,
                        GRW = !isEmu ? new() : null,
                        RW = !isEmu ? new() : null,
                        WZ = new(),
                        YW = !isEmu ? new() : null,
                        YZ = !isEmu ? new() : null
                    }
                });
            }
            _databaseService.UpdateTrainNumber(_databaseService.GetTrainNumberByNumber(number), timeTable, sections);
        }
        ProgressValue = 0;
        await _databaseService.SaveData();
        RefreshTrainNumbers();
    }
    [RelayCommand]
    public async Task TimeTableImportFromInternet() 
    {
        if (TimeTable.Count != 0)
        {
            if (!_dialogService.GetConfirm("当前操作可能清空时刻表。是否继续？"))
            {
                return;
            }
        }

        var stops = await _networkService.GetTimeTableAsync(SelectedTrainNumber.Number);
        if (stops != null)
        {
            TimeTable.Clear();
            foreach (TrainStop t in stops)
            {
                TimeTable.Add(t);
            }
            RefreshSections();
        }
        else
        {
            _dialogService.ShowMessage("获取失败", "未找到该车次。");
        }
    }
    [RelayCommand]
    public void AddTrainStop(object selectedTrainStop)
    {
        var newTrainStop = _dialogService.GetInputTrainNumberStop(null);
        if (newTrainStop != null)
        {
            if (TimeTable.Any(x => x.Station == newTrainStop.Station))
            {
                _dialogService.ShowMessage("错误", $"车站 {newTrainStop.Station} 已存在。");
            }
            if (selectedTrainStop != null)
            {
                TimeTable.Insert(TimeTable.IndexOf((TrainStop)selectedTrainStop) + 1, newTrainStop);
            }
            else
            {
                TimeTable.Add(newTrainStop);
            }
            RefreshSections();
        }
    }
    [RelayCommand]
    public void DeleteTrainStop(object selectedTrainStop)
    {
        if(selectedTrainStop is TrainStop s)
        {
            TimeTable.Remove(s);
            RefreshSections();
        }
    }
    [RelayCommand]
    public void DeleteAllTrainStop()
    {
        if (!_dialogService.GetConfirm("当前操作会删除全部停站，是否继续？")) return;
        TimeTable.Clear();
        RefreshSections();
    }
    [RelayCommand]
    public void EditTrainStop(object _selectedTrainStop)
    {
        if (_selectedTrainStop is TrainStop selectedTrainStop)
        {
            var newTrainStop = _dialogService.GetInputTrainNumberStop(selectedTrainStop);
            if (newTrainStop != null)
            {
                TimeTable[TimeTable.IndexOf(selectedTrainStop)] = newTrainStop;
            }
        }
    }
    [RelayCommand]
    public void TimeTableExportToExcel()
    {
        var path = _dialogService.SaveFile("Excel 工作薄 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*", "data");
        if (path == null) return;
        ExcelPackage.License.SetNonCommercialPersonal("CRSim");
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        worksheet.Cells[1, 1].Value = "车站名";
        worksheet.Cells[1, 2].Value = "到点";
        worksheet.Cells[1, 3].Value = "开点";
        for (int i = 0; i < TimeTable.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = TimeTable[i].Station;
            worksheet.Cells[i + 2, 2].Value = TimeTable[i].ArrivalTime?.ToString(@"hh\:mm") ?? string.Empty;
            worksheet.Cells[i + 2, 3].Value = TimeTable[i].DepartureTime?.ToString(@"hh\:mm") ?? string.Empty;
        }
        package.SaveAs(new FileInfo(path));
    }
    [RelayCommand]
    public void TimeTableImportFromExcel()
    {
        if (TimeTable.Count != 0)
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
            TimeTable.Clear();
            for (int row = 2; row <= rowCount; row++)
            {
                if (worksheet.Cells[row, 1].Text.Trim() == "" ||
                    (!TimeSpan.TryParseExact(worksheet.Cells[row, 2].Text.Trim(), @"hh\:mm", null, out _) &&
                    !TimeSpan.TryParseExact(worksheet.Cells[row, 3].Text.Trim(), @"hh\:mm", null, out _)))
                {
                    continue;
                }

                TimeTable.Add(new TrainStop
                {
                    Station = worksheet.Cells[row, 1].Text.Trim(),
                    ArrivalTime = TimeSpan.TryParseExact(worksheet.Cells[row, 2].Text.Trim(), @"hh\:mm", null, out TimeSpan arrival) ? arrival : null,
                    DepartureTime = TimeSpan.TryParseExact(worksheet.Cells[row, 3].Text.Trim(), @"hh\:mm", null, out TimeSpan departure) ? departure:null
                });
            }
            RefreshSections();
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    #endregion

    #region Sections
    [RelayCommand]
    public void SeatChecked(object? parameter)
    {
        if (parameter is string s)
        {
            foreach (var section in Sections)
            {
                var property = section.Tickets.GetType().GetProperty(s);
                if (property != null)
                {
                    var checkedProperty = GetType().GetProperty(s + "Checked");
                    bool @checked = (bool?)checkedProperty?.GetValue(this) ?? false;
                    property.SetValue(section.Tickets, @checked ? new Ticket() : null);
                }
            }
            RefreshSectionsUI();
        }

    }

    private void RefreshSections()
    {
        if (TimeTable.Count < 2) return;
        List<Section> oldSections = [.. Sections];
        Sections.Clear();
        for(int i=1;i< TimeTable.Count;i++)
        {
            if (oldSections.Any(x=>x.From== TimeTable[i - 1].Station&& x.To == TimeTable[i].Station))
            {
                Sections.Add(oldSections.FirstOrDefault(x => x.From == TimeTable[i - 1].Station && x.To == TimeTable[i].Station));
                continue;
            }
            Sections.Add(new Section
            {
                From = TimeTable[i - 1].Station,
                To = TimeTable[i].Station,
                Tickets = new Tickets
                {
                    SW = SWChecked ? new() : null,
                    ZYY = ZYYChecked ? new() : null,
                    ZY = ZYChecked ? new() : null,
                    ZE = ZEChecked ? new() : null,
                    RZ = RZChecked ? new() : null,
                    GRW = GRWChecked ? new() : null,
                    RW = RWChecked ? new() : null,
                    WZ = WZChecked ? new() : null,
                    YW = YWChecked ? new() : null,
                    YZ = YZChecked ? new() : null,
                }
            });
        }
    }
    #endregion

    #region Price
    [RelayCommand]
    public void SetPriceBatch(object s)
    {
        if (s is string type)
        {
            if (!_dialogService.GetConfirm("当前操作会覆盖现有车次配置，是否继续？")) return;
            TrainNumberSelected(null);

            Dictionary<(string, string, string), double> sectionPrices = [];
            string[] seatTypes = ["SW", "GRW", "YW", "ZE", "ZYY", "ZY", "WZ", "RW", "YZ", "RZ"];

            foreach (var trainNumber in TrainNumbers)
            {
                bool isHighSpeed = trainNumber.Number.StartsWith('G') || trainNumber.Number.StartsWith('D') || trainNumber.Number.StartsWith('C');
                if ((type == "1" && isHighSpeed) || (type == "2" && !isHighSpeed)) continue;

                foreach (var section in trainNumber.Sections)
                {
                    foreach (var seatType in seatTypes)
                    {
                        if (section.Tickets.GetType().GetProperty(seatType)?.GetValue(section.Tickets) is Ticket ticket && (!sectionPrices.ContainsKey((section.From, section.To, seatType)) || sectionPrices[(section.From, section.To, seatType)] == 0.0))
                        {
                            sectionPrices[(section.From, section.To, seatType)] = ticket.Price;
                        }
                    }
                }
            }

            foreach (var trainNumber in TrainNumbers)
            {
                bool isHighSpeed = trainNumber.Number.StartsWith('G') || trainNumber.Number.StartsWith('D') || trainNumber.Number.StartsWith('C');
                if ((type == "1" && isHighSpeed) || (type == "2" && !isHighSpeed)) continue;

                foreach (var section in trainNumber.Sections)
                {
                    foreach (var seatType in seatTypes)
                    {
                        if (section.Tickets.GetType().GetProperty(seatType)?.GetValue(section.Tickets) is Ticket ticket)
                        {
                            ticket.Price = sectionPrices[(section.From, section.To, seatType)];
                        }
                    }
                }
            }
        }
    }
    [RelayCommand]
    public void PriceImportFromExcel()
    {
        var path = _dialogService.GetFile("Excel 工作薄 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*");
        if (path == null) return;
        try
        {
            ExcelPackage.License.SetNonCommercialPersonal("CRSim");
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                var section = Sections.FirstOrDefault(x => x.From == worksheet.Cells[row, 1].Text.Trim() && x.To == worksheet.Cells[row, 2].Text.Trim());
                if (section != null)
                {
                    if (section.Tickets.SW != null)
                    {
                        section.Tickets.SW.Price = double.TryParse(worksheet.Cells[row, 3].Text.Trim(), out double sw) ? sw : 0.0;
                    }
                    if (section.Tickets.ZYY != null)
                    {
                        section.Tickets.ZYY.Price = double.TryParse(worksheet.Cells[row, 4].Text.Trim(), out double zyy) ? zyy : 0.0;
                    }
                    if (section.Tickets.ZY != null)
                    {
                        section.Tickets.ZY.Price = double.TryParse(worksheet.Cells[row, 5].Text.Trim(), out double zy) ? zy : 0.0;
                    }
                    if (section.Tickets.ZE != null)
                    {
                        section.Tickets.ZE.Price = double.TryParse(worksheet.Cells[row, 6].Text.Trim(), out double ze) ? ze : 0.0;
                    }
                    if (section.Tickets.GRW != null)
                    {
                        section.Tickets.GRW.Price = double.TryParse(worksheet.Cells[row, 7].Text.Trim(), out double grw) ? grw : 0.0;
                    }
                    if (section.Tickets.RW != null)
                    {
                        section.Tickets.RW.Price = double.TryParse(worksheet.Cells[row, 8].Text.Trim(), out double rw) ? rw : 0.0;
                    }
                    if (section.Tickets.YW != null)
                    {
                        section.Tickets.YW.Price = double.TryParse(worksheet.Cells[row, 9].Text.Trim(), out double yw) ? yw : 0.0;
                    }
                    if (section.Tickets.RZ != null)
                    {
                        section.Tickets.RZ.Price = double.TryParse(worksheet.Cells[row, 10].Text.Trim(), out double rz) ? rz : 0.0;
                    }
                    if (section.Tickets.YZ != null)
                    {
                        section.Tickets.YZ.Price = double.TryParse(worksheet.Cells[row, 11].Text.Trim(), out double yz) ? yz : 0.0;
                    }
                    if (section.Tickets.WZ != null)
                    {
                        section.Tickets.WZ.Price = double.TryParse(worksheet.Cells[row, 12].Text.Trim(), out double wz) ? wz : 0.0;
                    }
                }
            }
            RefreshSectionsUI();
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    [RelayCommand]
    public void PriceExportToExcel()
    {
        var path = _dialogService.SaveFile("Excel 工作薄 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*", "data");
        if (path == null) return;
        ExcelPackage.License.SetNonCommercialPersonal("CRSim");
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        worksheet.Cells[1, 1].Value = "区间起点";
        worksheet.Cells[1, 2].Value = "区间终点";
        worksheet.Cells[1, 3].Value = "商务座 特等座";
        worksheet.Cells[1, 4].Value = "优选一等座";
        worksheet.Cells[1, 5].Value = "一等座";
        worksheet.Cells[1, 6].Value = "二等座 二等包座";
        worksheet.Cells[1, 7].Value = "高级软卧";
        worksheet.Cells[1, 8].Value = "软卧/动卧 一等卧";
        worksheet.Cells[1, 9].Value = "硬卧 二等卧";
        worksheet.Cells[1, 10].Value = "软座";
        worksheet.Cells[1, 11].Value = "硬座";
        worksheet.Cells[1, 12].Value = "无座";
        for (int i = 0; i < Sections.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = Sections[i].From;
            worksheet.Cells[i + 2, 2].Value = Sections[i].To;
            worksheet.Cells[i + 2, 3].Value = Sections[i].Tickets.SW?.Price;
            worksheet.Cells[i + 2, 4].Value = Sections[i].Tickets.ZYY?.Price;
            worksheet.Cells[i + 2, 5].Value = Sections[i].Tickets.ZY?.Price;
            worksheet.Cells[i + 2, 6].Value = Sections[i].Tickets.ZE?.Price;
            worksheet.Cells[i + 2, 7].Value = Sections[i].Tickets.GRW?.Price;
            worksheet.Cells[i + 2, 8].Value = Sections[i].Tickets.RW?.Price;
            worksheet.Cells[i + 2, 9].Value = Sections[i].Tickets.YW?.Price;
            worksheet.Cells[i + 2, 10].Value = Sections[i].Tickets.RZ?.Price;
            worksheet.Cells[i + 2, 11].Value = Sections[i].Tickets.WZ?.Price;
            worksheet.Cells[i + 2, 12].Value = Sections[i].Tickets.YZ?.Price;
        }
        package.SaveAs(new FileInfo(path));
    }
    #endregion

    #region Quantity
    [RelayCommand]
    public void SetQuantityBatch(object s)
    {
        if(s is string extent)
        {
            if (!_dialogService.GetConfirm("当前操作会覆盖现有车次配置，是否继续？")) return;
            TrainNumberSelected(null);
            foreach(var trainNumber in TrainNumbers)
            {
                foreach(var Section in trainNumber.Sections)
                {
                    if (Section.Tickets.SW != null) Section.Tickets.SW.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.GRW != null) Section.Tickets.GRW.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.YW != null) Section.Tickets.YW.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.ZE != null) Section.Tickets.ZE.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.ZYY != null) Section.Tickets.ZYY.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.ZY != null) Section.Tickets.ZY.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.WZ != null) Section.Tickets.WZ.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.RW != null) Section.Tickets.RW.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.YZ != null) Section.Tickets.YZ.Quantity = RandomQuantity(extent);
                    if (Section.Tickets.RZ != null) Section.Tickets.RZ.Quantity = RandomQuantity(extent);
                }
            }
        }
    }
    private static int RandomQuantity(string extent)
    {
        switch (extent)
        {
            case "0":
                return (new Random()).Next(10, 100);
            case "1":
                return (new Random()).Next(0, 25);
            case "2":
                return (new Random()).Next(0, 2) == 0 ? (new Random()).Next(1, 5) : 0;
            default:
                break;
        }
        return 0;
    }
    [RelayCommand]
    public void QuantityImportFromExcel()
    {
        var path = _dialogService.GetFile("Excel 工作薄 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*");
        if (path == null) return;
        try
        {
            ExcelPackage.License.SetNonCommercialPersonal("CRSim");
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                var section = Sections.FirstOrDefault(x => x.From == worksheet.Cells[row, 1].Text.Trim() && x.To == worksheet.Cells[row, 2].Text.Trim());
                if (section != null)
                {
                    if (section.Tickets.SW != null)
                    {
                        section.Tickets.SW.Quantity = int.TryParse(worksheet.Cells[row, 3].Text.Trim(), out int sw) ? sw : 0;
                    }
                    if (section.Tickets.ZYY != null)
                    {
                        section.Tickets.ZYY.Quantity = int.TryParse(worksheet.Cells[row, 4].Text.Trim(), out int zyy) ? zyy : 0;
                    }
                    if (section.Tickets.ZY != null)
                    {
                        section.Tickets.ZY.Quantity = int.TryParse(worksheet.Cells[row, 5].Text.Trim(), out int zy) ? zy : 0;
                    }
                    if (section.Tickets.ZE != null)
                    {
                        section.Tickets.ZE.Quantity = int.TryParse(worksheet.Cells[row, 6].Text.Trim(), out int ze) ? ze : 0;
                    }
                    if (section.Tickets.GRW != null)
                    {
                        section.Tickets.GRW.Quantity = int.TryParse(worksheet.Cells[row, 7].Text.Trim(), out int grw) ? grw : 0;
                    }
                    if (section.Tickets.RW != null)
                    {
                        section.Tickets.RW.Quantity = int.TryParse(worksheet.Cells[row, 8].Text.Trim(), out int rw) ? rw : 0;
                    }
                    if (section.Tickets.YW != null)
                    {
                        section.Tickets.YW.Quantity = int.TryParse(worksheet.Cells[row, 9].Text.Trim(), out int yw) ? yw : 0;
                    }
                    if (section.Tickets.RZ != null)
                    {
                        section.Tickets.RZ.Quantity = int.TryParse(worksheet.Cells[row, 10].Text.Trim(), out int rz) ? rz : 0;
                    }
                    if (section.Tickets.YZ != null)
                    {
                        section.Tickets.YZ.Quantity = int.TryParse(worksheet.Cells[row, 11].Text.Trim(), out int yz) ? yz : 0;
                    }
                    if (section.Tickets.WZ != null)
                    {
                        section.Tickets.WZ.Quantity = int.TryParse(worksheet.Cells[row, 12].Text.Trim(), out int wz) ? wz : 0;
                    }
                }
            }
            RefreshSectionsUI();
        }
        catch
        {
            _dialogService.ShowMessage("导入失败", "文件格式错误或被占用。");
        }
    }
    [RelayCommand]
    public void QuantityExportToExcel()
    {
        var path = _dialogService.SaveFile("Excel 工作薄 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*", "data");
        if (path == null) return;
        ExcelPackage.License.SetNonCommercialPersonal("CRSim");
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        worksheet.Cells[1, 1].Value = "区间起点";
        worksheet.Cells[1, 2].Value = "区间终点";
        worksheet.Cells[1, 3].Value = "商务座 特等座";
        worksheet.Cells[1, 4].Value = "优选一等座";
        worksheet.Cells[1, 5].Value = "一等座";
        worksheet.Cells[1, 6].Value = "二等座 二等包座";
        worksheet.Cells[1, 7].Value = "高级软卧";
        worksheet.Cells[1, 8].Value = "软卧/动卧 一等卧";
        worksheet.Cells[1, 9].Value = "硬卧 二等卧";
        worksheet.Cells[1, 10].Value = "软座";
        worksheet.Cells[1, 11].Value = "硬座";
        worksheet.Cells[1, 12].Value = "无座";
        for (int i = 0; i < Sections.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = Sections[i].From;
            worksheet.Cells[i + 2, 2].Value = Sections[i].To;
            worksheet.Cells[i + 2, 3].Value = Sections[i].Tickets.SW?.Quantity;
            worksheet.Cells[i + 2, 4].Value = Sections[i].Tickets.ZYY?.Quantity;
            worksheet.Cells[i + 2, 5].Value = Sections[i].Tickets.ZY?.Quantity;
            worksheet.Cells[i + 2, 6].Value = Sections[i].Tickets.ZE?.Quantity;
            worksheet.Cells[i + 2, 7].Value = Sections[i].Tickets.GRW?.Quantity;
            worksheet.Cells[i + 2, 8].Value = Sections[i].Tickets.RW?.Quantity;
            worksheet.Cells[i + 2, 9].Value = Sections[i].Tickets.YW?.Quantity;
            worksheet.Cells[i + 2, 10].Value = Sections[i].Tickets.RZ?.Quantity;
            worksheet.Cells[i + 2, 11].Value = Sections[i].Tickets.WZ?.Quantity;
            worksheet.Cells[i + 2, 12].Value = Sections[i].Tickets.YZ?.Quantity;
        }
        package.SaveAs(new FileInfo(path));
    }
    #endregion

    [RelayCommand]
    public async Task SaveChanges()
    {
        if (!(await Validate())) return;
        _databaseService.UpdateTrainNumber(SelectedTrainNumber, [.. TimeTable], [.. Sections]);
        await _databaseService.SaveData();
    }

    private Task<bool> Validate()
    {
        string message = "";
        if (TimeTable.Count == 0)
        {
            return Task.FromResult(false);
        }
        if (TimeTable.First().ArrivalTime != null)
        {
            message += "\n时刻表首位必须是始发站；";
        }
        if (TimeTable.Last().DepartureTime != null)
        {
            message += "\n时刻表末位必须是终到站；";
        }
        if (TimeTable.Select(x => x.Station).GroupBy(x => x).Any(g => g.Count() > 1))
        {
            message += "\n存在多个重复停站；";
        }
        if (TimeTable.Count(x => x.ArrivalTime == null)!=1)
        {
            message += "\n存在多个始发站；";
        }
        if (TimeTable.Count(x => x.DepartureTime == null) != 1)
        {
            message += "\n存在多个终到站；";
        }
        if (message == "") return Task.FromResult(true);
        _dialogService.ShowMessage("保存失败", $"发现 {message.Split("\n").Length - 1} 个错误：{message}\n请修复所有错误后再次尝试保存。");
        return Task.FromResult(false);
    }

    private static List<string> ReadCsvFirstColumn(string filePath)
    {
        var firstColumnData = new List<string>();

        // 使用 StreamReader 读取文件
        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine(); // 读取每一行
                var columns = line.Split(','); // 按照逗号分隔列

                if (columns.Length > 0)
                {
                    var firstColumn = columns[0].Trim(); // 获取并去除首尾空格

                    if (!string.IsNullOrEmpty(firstColumn) && (char.IsLetterOrDigit(firstColumn[0])))
                    {
                        int spaceIndex = firstColumn.IndexOf(' ');
                        if (spaceIndex != -1)
                        {
                            firstColumn = firstColumn[..spaceIndex]; // 取空格前的部分
                        }

                        if (firstColumnData.Contains(firstColumn)) continue;
                        firstColumnData.Add(firstColumn); // 添加到结果列表
                    }
                }
            }
        }
        return firstColumnData;
    }

    public void RefreshSectionsUI()
    {
        var temp = Sections.ToList();
        Sections.Clear();            
        foreach (var item in temp)
        {
            Sections.Add(item);      
        }
    }
}