using System.Timers;

namespace CRSim.Views
{
    // 定义事件委托类型
    public delegate void SplashScreenClosedEventHandler(object sender, EventArgs e);

    public sealed partial class StartWindow : Window
    {
        // 声明事件
        public event SplashScreenClosedEventHandler SplashScreenClosed;

        // 进度条相关变量
        private readonly Timer _progressTimer;
        private double _currentProgress = 0;
        private const double ProgressIncrement = 1; // 每次增加的进度值
        private const int TimerInterval = 50; // 定时器间隔(毫秒)

        public StartWindow()
        {
            this.InitializeComponent();

            // 初始化定时器
            _progressTimer = new Timer(TimerInterval);
            _progressTimer.Elapsed += OnProgressTimerElapsed;
            _progressTimer.Start();
        }

        // 定时器事件处理：更新进度条
        private void OnProgressTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // 确保在UI线程更新
            this.DispatcherQueue.TryEnqueue(() =>
            {
                // 增加进度，最大到90%（留10%给最后完成）
                _currentProgress += ProgressIncrement;
                if (_currentProgress > 90)
                {
                    _currentProgress = 90;
                }

                progressBar.Value = _currentProgress;
            });
        }

        // 完成初始化，结束进度条并关闭
        public void CompleteInitialization()
        {
            // 停止定时器
            _progressTimer.Stop();
            _progressTimer.Dispose();

            // 快速将进度条拉满
            this.DispatcherQueue.TryEnqueue(() =>
            {
                progressBar.Value = 100;
            });

            // 短暂延迟后关闭，让用户看到进度完成
            Task.Delay(300).ContinueWith(_ =>
            {
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    SplashScreenClosed?.Invoke(this, EventArgs.Empty);
                    this.Close();
                });
            });
        }
    }
}
