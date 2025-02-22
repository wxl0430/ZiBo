using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRSim.Core.Models
{
    public class Station
    {
        public string Name { get; set; }

        public List<WaitingArea> WaitingAreas { get; set; } = [];

        [JsonIgnore]
        public List<string> TicketChecks
        {
            get
            {
                List<string> ticketChecks = [];
                ticketChecks.AddRange([.. WaitingAreas.SelectMany(wa => wa.TicketChecks)]);
                return ticketChecks;
            }
        }
        
        public List<StationStop> StationStops { get; set; } = [];

        public List<string> Platforms { get; set; } = [];
    }
}
