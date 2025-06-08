using CRSim.Core.Services;
using CRSim.Core.Models;
namespace CRSim.ScreenSimulator.ViewModels.Shanghai
{
    public class OutsideScreenViewModel : ScreenViewModel
    {
        public OutsideScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 9;
            ScreenCount = 1;
            StationType = StationType.Departure;
        }
    }
}
