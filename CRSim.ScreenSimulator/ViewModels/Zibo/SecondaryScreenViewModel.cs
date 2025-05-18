using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Zibo
{
    public class SecondaryScreenViewModel : ScreenViewModel
    {
        public SecondaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"开车前{settingsService.GetSettings().StopCheckInAdvanceDuration.TotalMinutes}分钟停止检票";
            ItemsPerPage = 18;
            PageCount = 1;
            StationType = StationType.Departure;
        }
    }
}
