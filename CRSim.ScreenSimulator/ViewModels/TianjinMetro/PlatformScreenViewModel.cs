using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.TianjinMetro
{
    public class PlatformScreenViewModel : MetroPlatformScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "海河明珠闪耀津门，古今交融绘就华章。五大道镌刻百年风华，滨海新区涌动创新浪潮。狗不理飘香，相声茶馆笑声扬。意式风情街浪漫流淌，古文化街非遗绽放。生态宜居城，开放包容乡。天津卫，邀您共谱新时代乐章！";
            Video = new("Assets\\Advertisement-3.mp4", UriKind.Relative);
        }
    }
}
