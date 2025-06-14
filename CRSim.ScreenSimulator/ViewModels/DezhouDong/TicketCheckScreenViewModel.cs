using CRSim.Core.Services;
using CRSim.Core.Models;
namespace CRSim.ScreenSimulator.ViewModels.DezhouDong
{
    public class TicketCheckScreenViewModel : ScreenViewModel
    {
        public TicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 3;
            StationType = StationType.Departure;
        }
    }
}
