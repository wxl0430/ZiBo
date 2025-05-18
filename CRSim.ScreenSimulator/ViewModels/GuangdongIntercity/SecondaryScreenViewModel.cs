using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.GuangdongIntercity
{
    public class SecondaryScreenViewModel : ScreenViewModel
    {
        public SecondaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = $"列 车 到 发 信 息";
            ItemsPerPage = 8;
            PageCount = 1;
        }
    }
}
