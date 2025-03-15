using CRSim.Core.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRSim.Core.Models
{
    public class TrainStop
    {
        public string Number { get; set; }

        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public TimeSpan? ArrivalTime { get; set; }

        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public TimeSpan? DepartureTime { get; set; }

        public List<string> TicketChecks { get; set; }

        public string Origin { get; set; }

        public string Terminal { get; set; }

        public string Platform { get; set; }

        public string? Landmark { get; set; }

        public int Length { get; set; }

        [JsonIgnore]
        public StationType StationType
        {
            get
            {
                if (ArrivalTime == null)
                {
                    return StationType.Departure;
                }
                else if (DepartureTime == null)
                {
                    return StationType.Arrival;
                }
                return StationType.Both;
            }
        }
    }
}
