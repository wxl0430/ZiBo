using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Hanzhong
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            ItemsPerPage = 1;
            Text = "请注意列车与站台之间的间隙，请勿携带危险品上车";
        }
    }
}