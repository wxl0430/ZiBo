using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Hailaer
{
    public class PrimaryScreenViewModel : ScreenViewModel
    {
        public PrimaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 6;
            ScreenCount = 2;
            Text = $"    开车前{settingsService.GetSettings().StopCheckInAdvanceDuration.TotalMinutes}分钟停止检票";;
            StationType = StationType.Departure;
        }
    }
}
