using System.Reflection;
using System.Timers;
using Windows.Graphics;

namespace CRSim.Views
{
    // 定义事件委托类型
    public delegate void SplashScreenClosedEventHandler(object sender, EventArgs e);

    public sealed partial class StartWindow : Window
    {
        // 声明事件
        public event SplashScreenClosedEventHandler SplashScreenClosed;
        public string AppVersion { get; set; } = Assembly.GetAssembly(typeof(App)).GetName().Version.ToString();

        public StartWindow()
        {
            this.InitializeComponent();

            AppWindow.Resize(new Windows.Graphics.SizeInt32(750, 270));
            var area = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)?.WorkArea;
            if (area != null)
            {
                AppWindow.Move(new PointInt32((area.Value.Width - AppWindow.Size.Width) / 2, (area.Value.Height - AppWindow.Size.Height) / 2));
            }


            OverlappedPresenter presenter = OverlappedPresenter.Create();

            presenter.IsAlwaysOnTop = true;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(true, false);

            AppWindow.SetPresenter(presenter);

            AppWindow.MoveInZOrderAtTop();

            Logo.MediaPlayer.Play();
            

        }

        // 完成初始化，结束进度条并关闭
        public void CompleteInitialization()
        {
            SplashScreenClosed?.Invoke(this, EventArgs.Empty);
            this.Close();
        }
    }
}
