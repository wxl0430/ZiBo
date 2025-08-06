using System.Reflection;
using Windows.ApplicationModel;

namespace CRSim
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }
        public static MainWindow MainWindow;
        public string[] commandLineArgs;
        public CommandLineOptions parsedOptions;
        public static string AppVersion { get; set; } = Assembly.GetAssembly(typeof(App)).GetName().Version.ToString(); 
        public static DispatcherQueue DispatcherQueue { get; private set; }

        public App()
        {
            var args = Environment.GetCommandLineArgs();
            parsedOptions = CommandLineParser.Parse(args);
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

            DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ShowSplashScreen();
        }

        private void ShowSplashScreen()
        {            
            var splashScreenWindow = AppHost.Services.GetService<StartWindow>();
            splashScreenWindow.SplashScreenClosed += OnSplashScreenClosed;
            splashScreenWindow.Activate();
            _ = PerformInitializationAsync(splashScreenWindow);
        }

        private async Task PerformInitializationAsync(StartWindow splashScreen)
        {
            try
            {
                await LoadPluginService();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化错误: {ex.Message}");
            }
            finally
            {
                splashScreen.CompleteInitialization();
            }
        }

        private void OnSplashScreenClosed(object sender, EventArgs e)
        {
            var mainWindow = AppHost.Services.GetService<MainWindow>();
            mainWindow.Activate();
            MainWindow = mainWindow;
        }

        private Task LoadPluginService()
        {
            return Task.Delay(5600);
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
            Console.WriteLine("    CRSim - 国铁信息显示模拟 v" + AppVersion);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Debug console started.\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
