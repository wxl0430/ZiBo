using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.DaqingDong
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 1;
            Text = $"请站在白色安全线以内排队候车";
        }
    }
}
