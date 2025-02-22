using CRSim.Core.Models;

namespace CRSim.Core.Services
{
    public interface IDatabaseService
    {
        List<Station> GetAllStations();
        List<TrainNumber> GetAllTrainNumbers();
        Station GetStationByName(string name);
        TrainNumber GetTrainNumberByNumber(string number);
        void AddStationByName(string name);
        void AddStation(Station station);
        Task ImportStationFrom7D(string path);
        void AddTrainNumber(string number);
        void DeleteStation(Station station);
        void DeleteTrainNumber(TrainNumber trainNumber);
        void UpdateStation(string stationName,Station station);
        void UpdateTimeTable(TrainNumber trainNumber, List<TrainStop> trainStopsList);
        Task SaveData();
        void ImportData(string jsonFilePath);
        Task ExportData(string path);
        void ClearData();
    }
}
