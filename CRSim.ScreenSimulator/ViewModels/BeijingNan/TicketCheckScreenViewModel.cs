using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.BeijingNan
{
    public class TicketCheckScreenViewModel : ScreenViewModel
    {
        public TicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 2;
            StationType = StationType.Departure;
        }
    }
}
