using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.BeijingNan
{
    public class PrimaryPlatformScreenViewModel : ScreenViewModel
    {
        public PrimaryPlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 1;
        }
    }
}
