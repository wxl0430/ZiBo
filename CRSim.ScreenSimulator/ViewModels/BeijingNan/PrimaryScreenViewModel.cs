using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.BeijingNan
{
    public class PrimaryScreenViewModel : ScreenViewModel
    {
        public PrimaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 10;
            ScreenCount = 1;
            StationType = StationType.Departure;
        }
    }
}
