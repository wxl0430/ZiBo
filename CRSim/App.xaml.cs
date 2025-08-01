namespace CRSim
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }
        public static MainWindow MainWindow;
        public string[] commandLineArgs;

        public App()
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

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = AppHost.Services.GetService<MainWindow>();
            MainWindow?.Activate();
            AppHost.Services.GetService<StyleManager>();//init
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
