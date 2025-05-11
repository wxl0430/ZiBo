namespace CRSim.ScreenSimulator.Models
{
    public class ScreenInfo
    {
        public required Guid SessionID { get; set; }
        public required string SelectedStyle { get; set; }
        public string? Text { get; set; }
        public string? Video { get; set; }
        public string? SelectedStationName { get; set; }
        public string? SelectedTicketCheck { get; set; }
        public string? SelectedPlatformName { get; set; }
        public int? SelectedLoaction { get; set; }
    }
}
