using CRSim.Core.Models;
using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.ChengduDong
{
    public class PrimaryTicketCheckScreenViewModel : ScreenViewModel
    {
        public PrimaryTicketCheckScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "持户口簿、护照、临时身份证的旅客，请在非居民身份证通道排队。请主动礼让老、弱、病、残、孕的旅客。";
            ItemsPerPage = 2;
            StationType = StationType.Departure;
        }
    }
}
