using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.ChengduShuangliuAirport
{
    public class SecondaryScreenViewModel : ScreenViewModel
    {
        public SecondaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"国内航班截载时间为起飞前{settingsService.GetSettings().StopCheckInAdvanceDuration.TotalMinutes}分钟。";
            ItemsPerPage = 7;
            ScreenCount = 1;
            StationType = StationType.Departure;
        }
    }
}
