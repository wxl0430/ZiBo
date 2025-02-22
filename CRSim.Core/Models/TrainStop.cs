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
        public string Station { get; set; }

        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public DateTime? ArrivalTime { get; set; }

        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public DateTime? DepartureTime { get; set; }
    }
}
