using CRSim.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSim.Core.Services
{
    public interface ISettingsService
    {
        void SaveSettings();
        Settings GetSettings();
    }
}
