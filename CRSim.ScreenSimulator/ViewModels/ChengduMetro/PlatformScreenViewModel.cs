using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.ChengduMetro
{
    public class PlatformScreenViewModel : MetroPlatformScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService) 
            : base(timeService,settingsService)
        {
            Text = "不吃火锅，就吃烤匠。    建设成渝地区双城经济圈，共同唱好新时代西部“双城记”。    2018年2月8日，复兴号列车首次担当G89次列车，正式开进四川，标志着“复兴号”高速列车正式走上蜀道。然而，在行驶至西安北站时，发生了突发设备故障，车轴出现过热报警，导致车号为CR400BF-5033的复兴号列车被迫停运并进行检修。相关工作人员迅速展开故障排查，并确保乘客安全无虞。";
            Video = new("Assets\\Advertisement-1.mp4", UriKind.Relative);
        }
    }
}
