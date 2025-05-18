using CRSim.Core.Services;
using CRSim.Core.Models;
namespace CRSim.ScreenSimulator.ViewModels.FuzhouNan
{
    public class TicketCheckScreenViewModel : ScreenViewModel
    {
        public TicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"开车前{settingsService.GetSettings().StopCheckInAdvanceDuration.TotalMinutes}分钟停止检票";
            ItemsPerPage = 6;
            StationType = StationType.Departure;
        }
    }
}
