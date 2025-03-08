namespace CRSim.Services
{
    public interface IDialogService
    {
        string? GetFile(string filter);
        string? SaveFile(string filter,string fileName);
        string? GetInput(string title);
        bool GetConfirm(string title);
        void ShowMessage(string title, string message);
        TrainStop? GetInputTrainStop();
        TrainStop? GetInputTrainStop(TrainStop trainStop);
        StationStop? GetInputStationStop(List<string> ticketChecks, List<string> platforms);
        StationStop? GetInputStationStop(List<string> ticketChecks, List<string> platforms, StationStop stationStop);
        List<Platform>? GetInputPlatform();
        (string, List<string>) GetInputTicketCheck(List<string> waitingAreaNames);
        void SetOwner(Window owner);
    }
}
