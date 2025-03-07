using CRSim.Core.Models;
using System.Text.Json;

namespace CRSim.Core.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _jsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CRSim");
        private List<Station> _stations;
        private List<TrainNumber> _trainNumbers;

        private readonly JsonSerializerOptions options = new() { WriteIndented = true };

        public DatabaseService()
        {
            if (!Directory.Exists(_jsonFilePath)) Directory.CreateDirectory(_jsonFilePath);
            _jsonFilePath += "\\data.json";
            ImportData(_jsonFilePath);
        }

        public List<Station> GetAllStations()
        {
            return _stations;
        }

        public void ImportData(string jsonFilePath)
        {
            try
            {
                var json =  File.ReadAllText(jsonFilePath);
                var data = JsonSerializer.Deserialize<Json>(json);
                _stations = data.Stations;
                _trainNumbers = data.TrainNumbers;
            }
            catch
            {
                _stations = [];
                _trainNumbers = [];
            }
        }
        public Station GetStationByName(string name)
        {
            return _stations.FirstOrDefault(x => x.Name == name);
        }
        public TrainNumber GetTrainNumberByNumber(string number)
        {
            return _trainNumbers.FirstOrDefault(x => x.Number == number);
        }
        public void AddStationByName(string name)
        {
            _stations.Add(new Station()
            {
                Name = name,
            });
        }

        public void DeleteStation(Station station)
        {
            _stations.Remove(station);
        }

        public void DeleteTrainNumber(TrainNumber trainNumber)
        {
            _trainNumbers.Remove(trainNumber);
        }

        public void UpdateStation(string stationName,Station station)
        {
            var targetStation = _stations.FirstOrDefault(x => x.Name == stationName);
            if (targetStation != null)
            {
                // 直接更新 targetStation 中的内容
                targetStation.Name = station.Name;
                targetStation.WaitingAreas = station.WaitingAreas;
                targetStation.StationStops = station.StationStops;
                targetStation.Platforms = station.Platforms;
            }
        }

        public void UpdateTimeTable(TrainNumber trainNumber, List<TrainStop> trainStopsList)
        {
            var targetTrainNumber = _trainNumbers.FirstOrDefault(x => x.Number == trainNumber.Number);
            targetTrainNumber.TimeTable = trainStopsList;
        }

        public async Task SaveData()
        {
            string json = JsonSerializer.Serialize(new Json()
            { 
                Stations = _stations,
                TrainNumbers = _trainNumbers
            }, options);
            await File.WriteAllTextAsync(_jsonFilePath, json);
        }

        public async Task ExportData(string p)
        {
            string json = JsonSerializer.Serialize(new Json()
            {
                Stations = _stations,
                TrainNumbers = _trainNumbers
            }, options);
            await File.WriteAllTextAsync(p, json);
        }

        public List<TrainNumber> GetAllTrainNumbers()
        {
            return _trainNumbers;
        }

        public void AddTrainNumber(string number)
        {
            _trainNumbers.Add(new TrainNumber()
            {
                Number = number
            });
        }

        public async Task ImportStationFrom7D(string path)
        {
            Station station = new() { Platforms = [], WaitingAreas = [], StationStops = [] };
            var stationName = (await File.ReadAllTextAsync(path.Replace("车站广播系统.exe", "info.ini"))).Split("|")[0];
            if (_stations.Any(x => x.Name == stationName))
            {
                _stations.Remove(GetStationByName(stationName));
            }
            var lines = await File.ReadAllLinesAsync(path.Replace("车站广播系统.exe", "data.csv"));
            station.Name = stationName;
            foreach (var line in lines)
            {
                var data = line.Split(',');
                var waitingAreaName = string.IsNullOrWhiteSpace(data.Last()) ? "未知" : data.Last();
                var platform = string.IsNullOrWhiteSpace(data[5]) ? "未知" : data[5];
                var ticketCheck = string.IsNullOrWhiteSpace(data[4]) ? string.Empty : data[4];

                if (!station.Platforms.Contains(platform))
                {
                    station.Platforms.Add(platform);
                }

                var waitingArea = station.WaitingAreas.FirstOrDefault(x => x.Name == waitingAreaName);
                if (waitingArea == null)
                {
                    waitingArea = new WaitingArea() { Name = waitingAreaName };
                    station.WaitingAreas.Add(waitingArea);
                }

                foreach (var ticketCheckName in ticketCheck.Split('|'))
                {
                    if (!string.IsNullOrWhiteSpace(ticketCheckName) && !waitingArea.TicketChecks.Contains(ticketCheckName))
                    {
                        waitingArea.TicketChecks.Add(ticketCheckName);
                    }
                }

                if (!string.IsNullOrEmpty(data[0]) && !station.StationStops.Any(x => x.Number == data[0]))
                {
                    DateTime? arrivalTime = null;
                    DateTime? departureTime = null;

                    if (data[2] == stationName)
                    {
                        departureTime = DateTime.Parse(lines.FirstOrDefault(x => x.Split(",")[0] == data[0] && x.Split(",")[1].Contains("送车"))?.Split(",")[7] ?? string.Empty);
                    }
                    else if (data[3] == stationName)
                    {
                        arrivalTime = DateTime.Parse(data[7]);
                    }
                    else
                    {
                        arrivalTime = DateTime.Parse(data[7]);
                        departureTime = DateTime.Parse(lines.FirstOrDefault(x => x.Split(",")[0] == data[0] && x.Split(",")[1].Contains("送车"))?.Split(",")[7] ?? string.Empty);
                    }

                    station.StationStops.Add(new StationStop
                    {
                        Number = data[0],
                        Terminal = data[3],
                        Origin = data[2],
                        TicketChecks = [.. ticketCheck.Split('|')],
                        ArrivalTime = arrivalTime,
                        DepartureTime = departureTime,
                        Platform = platform,
                        Landmark = data[8] + "色" ?? null,
                    });
                }
            }
            AddStation(station);
        }

        public void AddStation(Station station)
        {
            _stations.Add(station);
        }

        public void ClearData()
        {
            _stations.Clear();
            _trainNumbers.Clear();
        }
    }
}
