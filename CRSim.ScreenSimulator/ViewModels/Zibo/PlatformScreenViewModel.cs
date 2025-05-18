using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Zibo
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"请站在白色安全线以内排队候车";
            ItemsPerPage = 1;
        }
    }
}
