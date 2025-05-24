using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.BeijingNan
{
    public class SecondaryPlatformScreenViewModel : ScreenViewModel
    {
        public SecondaryPlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 1;
        }
    }
}
