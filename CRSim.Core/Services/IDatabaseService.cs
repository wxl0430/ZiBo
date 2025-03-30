using CRSim.Core.Models;
using static System.Collections.Specialized.BitVector32;
using Section = CRSim.Core.Models.Section;

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
        void UpdateTrainNumber(TrainNumber trainNumber, List<TrainStop> timeTable, List<Section>? sections);
        Task SaveData();
        void ImportData(string jsonFilePath);
        Task ExportData(string path);
        void ClearData();
    }
}
