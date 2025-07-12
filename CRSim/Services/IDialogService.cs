namespace CRSim.Services
{
    public interface IDialogService
    {
        Task<string?> GetFileAsync(string[] filter);
        Task<string?> SaveFileAsync(string filter,string fileName);
        Task<string?> GetInputAsync(string title);
        Task<bool> GetConfirmAsync(string title);
        Task ShowMessageAsync(string title, string message);
        Task ShowTextAsync(string title, string message);
        TrainStop? GetInputTrainNumberStop(TrainStop? t);
        Task<TrainStop?> GetInputTrainStopAsync(List<string> ticketChecks, List<string> platforms);
        Task<TrainStop?> EditInputTrainStopAsync(List<string> ticketChecks, List<string> platforms, TrainStop trainStop);
        Task<List<Platform>?> GetInputPlatformAsync();
        Task<(string, List<string>)> GetInputTicketCheckAsync(List<string> waitingAreaNames);
    }
}
