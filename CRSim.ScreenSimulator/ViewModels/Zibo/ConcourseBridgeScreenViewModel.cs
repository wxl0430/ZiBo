using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Zibo
{
    public class ConcourseBridgeScreenViewModel : ScreenViewModel
    {
        public ConcourseBridgeScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            StationType = StationType.Departure;
            ItemsPerPage = 2;
        }
    }
}
