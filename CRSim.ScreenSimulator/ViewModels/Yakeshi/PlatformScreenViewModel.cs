using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.Yakeshi
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"上下车不要拥挤，小心列车与站台之间的缝隙，看管好老人和同行的儿童";
            ItemsPerPage = 1;
        }
    }
}
