using CRSim.Core.Services;
namespace CRSim.ScreenSimulator.ViewModels.XuzhouMetro
{
    public class PlatformScreenViewModel : MetroPlatformScreenViewModel
    {
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            Text = "徐州，一座镌刻华夏文明印记的英雄之城。这里是大汉王朝的摇篮，楚风汉韵流淌在云龙山水间，龟山汉墓诉说着千年的传奇，汉画像石馆定格了古人的智慧光影。九里山前古战场，戏马台边秋风烈，历史的豪情从未褪色。作为五省通衢的枢纽，徐州以开放的胸怀连接南北，高铁飞驰、运河如练，现代脉动与古朴底蕴在此交融。登云龙山俯瞰一城青山半城湖，泛舟云龙湖感受水墨诗意；户部山的街巷藏着老徐州的烟火气，回龙窝的灯火点亮了夜色温柔。从地锅鸡的鲜香到饣它汤的醇厚，从酥脆的烙馍到清甜的蜜三刀，舌尖上的彭城满是热情滋味。这里是淮海之芯，用智造之光照亮未来，用楚汉气魄续写新章——徐州，等你共赴一场跨越时空的约定！";
            Video = new("Assets\\Advertisement-2.mp4", UriKind.Relative);
        }
    }
}
