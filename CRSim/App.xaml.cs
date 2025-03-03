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
                services.AddSingleton<ISettingsService,SettingsService>(sp => new SettingsService("app.config"));
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<IDatabaseService, DatabaseService>(sp => new DatabaseService("data.json"));
                services.AddSingleton<INetworkService, NetworkService>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddTransient<DashboardPage>();
                services.AddTransient<DashboardPageViewModel>();

                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsPageViewModel>();

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

                services.AddTransient<ViewModels.ChengduDong.SecondaryScreenViewModel>();
                services.AddTransient<Views.ChengduDong.SecondaryScreenView>();
                services.AddTransient<ViewModels.ChengduDong.PlatformScreenViewModel>();
                services.AddTransient<Views.ChengduDong.PrimaryTicketCheckScreenView>();
                services.AddTransient<ViewModels.BeijingXi.PrimaryScreenViewModel>();
                services.AddTransient<Views.BeijingXi.PrimaryScreenView>();
                services.AddTransient<ViewModels.Harbin.PrimaryScreenViewModel>();
                services.AddTransient<Views.Harbin.PrimaryScreenView>();
                services.AddTransient<ViewModels.Guangyuan.OutsideScreenViewModel>();
                services.AddTransient<Views.Guangyuan.PrimaryScreenView>();
                services.AddTransient<ViewModels.ChengduMetro.PlatformScreenViewModel>();
                services.AddTransient<Views.ChengduMetro.PlatformScreenView>();
                services.AddTransient<ViewModels.Shanghai.OutsideScreenViewModel>();
                services.AddTransient<Views.Shanghai.OutsideScreenView>();
                services.AddTransient<ViewModels.Hanzhong.PlatformScreenViewModel>();
                services.AddTransient<Views.Hanzhong.PlatformScreenView>();
                services.AddTransient<ViewModels.Zibo.PrimaryScreenViewModel>();
                services.AddTransient<Views.Zibo.PrimaryScreenView>();
                services.AddTransient<ViewModels.Zibo.SecondaryScreenViewModel>();
                services.AddTransient<Views.Zibo.SecondaryScreenView>();
                // services.AddTransient<ViewModels.Zibo.PrimaryTicketCheckScreenViewModel>();
                // services.AddTransient<Views.Zibo.PrimaryTicketCheckScreenView>();
                services.AddTransient<ViewModels.Jinanxi.PrimaryScreenViewModel>();
                services.AddTransient<Views.Jinanxi.PrimaryScreenView>();
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

            app.MainWindow.Visibility = Visibility.Visible;
            app.Run();
        }
    }

}
