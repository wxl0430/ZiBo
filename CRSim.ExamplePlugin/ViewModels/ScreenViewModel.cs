using CRSim.Core.Models;
using CRSim.Core.Abstractions;
using CRSim.ScreenSimulator.ViewModels;
namespace CRSim.ExamplePlugin.ViewModels
{
    public class ScreenViewModel : BaseScreenViewModel
    {
        public ScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "成都铁路客服电话：028-12306";
            ItemsPerPage = 12;
            ScreenCount = 1;
            StationType = StationType.Departure;
        }
    }
}
