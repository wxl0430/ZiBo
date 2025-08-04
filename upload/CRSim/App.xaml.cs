namespace CRSim
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }
        public static MainWindow MainWindow;
        public string[] commandLineArgs;
        public static DispatcherQueue DispatcherQueue { get; private set; }

        public App()
        {
            

            // 初始化DispatcherQueue
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // 显示启动窗口
            ShowSplashScreen();
        }

        // 显示启动画面
        private void ShowSplashScreen()
        {
            // 从依赖注入容器获取启动窗口实例
            var splashScreenWindow = AppHost.Services.GetService<StartWindow>();

            // 订阅启动窗口关闭事件
            splashScreenWindow.SplashScreenClosed += OnSplashScreenClosed;

            // 激活启动窗口
            splashScreenWindow.Activate();

            // 在启动窗口显示期间执行初始化任务
            _ = PerformInitializationAsync(splashScreenWindow);
        }

        // 执行应用初始化任务
        private async Task PerformInitializationAsync(StartWindow splashScreen)
        {
            try
            {
                // 执行实际初始化工作（替换为你的真实初始化逻辑）
                await InitializeDatabaseAsync();
                await LoadApplicationSettingsAsync();
                await InitializePluginsAsync();

                // 可以在这里更新启动画面进度
                // splashScreen.UpdateProgress(75); // 示例：更新进度到75%
            }
            catch (Exception ex)
            {
                // 处理初始化错误
                Console.WriteLine($"初始化错误: {ex.Message}");
            }
            finally
            {
                // 初始化完成，关闭启动画面
                splashScreen.CompleteInitialization();
            }
        }

        // 启动画面关闭时调用
        private void OnSplashScreenClosed(object sender, EventArgs e)
        {
            // 从依赖注入容器获取主窗口实例
            var mainWindow = AppHost.Services.GetService<MainWindow>();

            // 激活主窗口
            mainWindow.Activate();

            // 设置为应用的主窗口
            MainWindow = mainWindow;
        }

        // 模拟初始化方法（替换为实际实现）
        private Task InitializeDatabaseAsync()
        {
            var args = Environment.GetCommandLineArgs();
            var parsedOptions = CommandLineParser.Parse(args);
            if (parsedOptions.Debug)
            {
                LaunchDebugConsole();
            }
            if (!Directory.Exists(AppPaths.AppDataPath))
            {
                Directory.CreateDirectory(AppPaths.AppDataPath);
            }
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(parsedOptions);
                    services.AddSingleton<IPluginService, PluginService>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IDatabaseService, DatabaseService>();
                    services.AddSingleton<INetworkService, NetworkService>();
                    services.AddSingleton<IDialogService, DialogService>();
                    services.AddSingleton<StyleManager>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<StartWindow>();
                    services.AddTransient<ITimeService, TimeService>();
                    services.AddTransient<DashboardPageViewModel>();
                    services.AddTransient<StationManagementPageViewModel>();
                    services.AddTransient<ScreenSimulatorPageViewModel>();
                    services.AddTransient<SettingsPageViewModel>();
                    services.AddTransient<PluginManagementPageViewModel>();
                    PluginService.InitializePlugins(context, services, parsedOptions.ExternalPluginPath);
                })
            .Build();
            InitializeComponent();
        }

        private Task LoadApplicationSettingsAsync()
        {
            // 加载应用设置
            return Task.Delay(500);
        }

        private Task InitializePluginsAsync()
        {
            // 初始化插件系统
            return Task.Delay(5000);
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public static void LaunchDebugConsole()
        {
            AllocConsole();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
        ___ _____________________
   _.-''---'  |  ___ ___ ___ ___|
  (       __|_|_[___[___[___[__]  
 =-(_)--(_)--'      (O)   (O) ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("       CRSim - 国铁信息显示模拟");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Debug console started.\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
