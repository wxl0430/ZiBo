namespace CRSim.Models
{
    public class TrainInfo
    {
        public string TrainNumber { get; set; }
        public string Origin { get; set; }
        public string Terminal { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public List<string> TicketChecks { get; set; }
        public string WaitingArea { get; set; }
        public string Platform { get; set; }
        public TimeSpan State { get; set; }
        public string? Landmark { get; set; }
        public int Length { get; set; }
    }
}
