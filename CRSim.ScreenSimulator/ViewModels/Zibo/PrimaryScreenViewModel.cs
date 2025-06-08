using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Zibo
{
    public class PrimaryScreenViewModel : ScreenViewModel
    {
        public PrimaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 8;
            ScreenCount = 2;
            StationType = StationType.Departure;
        }
    }
}
