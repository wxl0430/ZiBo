using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSim.Core.Models
{
    public class WaitingArea
    {
        public string Name { get; set; }
        public List<string> TicketChecks { get; set; } = [];
    }
}
