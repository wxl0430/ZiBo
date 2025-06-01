using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.ShenyangBei
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 1;
        }
    }
}
