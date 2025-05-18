using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.ChengduDong
{
    public class SecondaryScreenViewModel : ScreenViewModel
    {
        public SecondaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "    检票口信息请以车站显示屏信息为准。公共场所请妥善保管个人贵重物品。";
            ItemsPerPage = 12;
            PageCount = 2;
            StationType = StationType.Departure;
        }
    }
}
