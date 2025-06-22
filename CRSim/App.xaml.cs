using CRSim.ScreenSimulator.Services;

namespace CRSim
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly IHost _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ISettingsService,SettingsService>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<IDatabaseService, DatabaseService>();
                services.AddSingleton<INetworkService, NetworkService>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddTransient<DashboardPage>();
                services.AddTransient<DashboardPageViewModel>();

                services.AddTransient<SettingsPage>();
                services.AddTransient<SettingsPageViewModel>();

                services.AddTransient<DataManagementPage>();
                services.AddTransient<DataManagementPageViewModel>();
                services.AddTransient<StationManagementPage>();
                services.AddTransient<StationManagementPageViewModel>();
                services.AddTransient<TrainNumberManagementPage>();
                services.AddTransient<TrainNumberManagementPageViewModel>();

                services.AddTransient<StartSimulationPage>();
                services.AddTransient<StartSimulationPageViewModel>();
                services.AddTransient<ScreenSimulationPage>();
                services.AddTransient<ScreenSimulationPageViewModel>();
                services.AddTransient<WebsiteSimulationPage>();
                services.AddTransient<WebsiteSimulationPageViewModel>();

                services.AddTransient<WebsiteSimulator.Simulator>();
                services.AddScreenSimulatorServices();

                services.AddTransient<UpdateChecker>();

            }).Build();

        [STAThread]
        public static void Main()
        {
            _host.Start();

            App app = new();
            app.InitializeComponent();
            Current.Resources["ServiceProvider"] = _host.Services;
            app.MainWindow = _host.Services.GetRequiredService<MainWindow>();

            _host.Services.GetRequiredService<ITimeService>().Start();
            _host.Services.GetRequiredService<IDialogService>().SetOwner(app.MainWindow);

            var updateChecker = new UpdateChecker();
            updateChecker.CheckForUpdates().Wait();
            
            app.MainWindow.Visibility = Visibility.Visible;
            app.Run();
        }
    }

}
