using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.HarbinXi
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"请在白线内行走，乘降列车时注意脚下站台与列车之间的空隙，看护好老人和儿童。";
            ItemsPerPage = 1;
        }
    }
}
