using CRSim.Core.Services;
using CRSim.Core.Models;
namespace CRSim.ScreenSimulator.ViewModels.Guangyuan
{
    public class PrimaryScreenViewModel : ScreenViewModel
    {
        public PrimaryScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "成都铁路客服电话：028-12306";
            ItemsPerPage = 12;
            ScreenCount = 1;
            StationType = StationType.Departure;
        }
    }
}
