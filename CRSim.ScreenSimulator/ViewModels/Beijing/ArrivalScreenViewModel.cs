using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Beijing
{
    public class ArrivalScreenViewModel : ScreenViewModel
    {
        public ArrivalScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 2;
            StationType = StationType.Arrival;
        }
    }
}
