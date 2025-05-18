using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.DaqingDong
{
    public class ConcourseBridgeScreenViewModel : ScreenViewModel
    {
        public ConcourseBridgeScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 3;
            StationType = StationType.Departure;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
        }
    }
}
