using CRSim.Core.Services;
using CRSim.Core.Models;
namespace CRSim.ScreenSimulator.ViewModels.Ankang
{
    public class ExitScreenViewModel : ScreenViewModel
    {
        public ExitScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"西安局集团公司推出西铁行APP站车服务产品，欢迎体验使用。";
            ItemsPerPage = 7;
            ScreenCount = 1;
            StationType = StationType.Arrival;
        }
    }
}
