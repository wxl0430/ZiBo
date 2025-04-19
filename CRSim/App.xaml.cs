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

                services.AddTransient<ScreenSimulator.ViewModels.ChengduDong.SecondaryScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.ChengduDong.SecondaryScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.ChengduDong.PrimaryTicketCheckScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.ChengduDong.PrimaryTicketCheckScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.BeijingXi.PrimaryScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.BeijingXi.PrimaryScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Harbin.PrimaryScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Harbin.PrimaryScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Guangyuan.OutsideScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Guangyuan.PrimaryScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.ChengduMetro.PlatformScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.ChengduMetro.PlatformScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.XuzhouMetro.PlatformScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.XuzhouMetro.PlatformScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Shanghai.OutsideScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Shanghai.OutsideScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Hanzhong.PlatformScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Hanzhong.PlatformScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Zibo.PrimaryScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Zibo.PrimaryScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Zibo.SecondaryScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Zibo.SecondaryScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Zibo.TicketCheckScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Zibo.TicketCheckScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Zibo.PlatformScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Zibo.PlatformScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Zibo.ConcourseBridgeScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Zibo.ConcourseBridgeScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Jinanxi.SmallScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Jinanxi.SmallScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.GuangdongIntercity.SecondaryScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.GuangdongIntercity.SecondaryScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Tianjin.PlatformScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Tianjin.PlatformScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Mianyang.PlatformScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Mianyang.PlatformScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Fuzhou.TicketCheckScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Fuzhou.TicketCheckScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.FuzhouNan.TicketCheckScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.FuzhouNan.TicketCheckScreenView>();
                services.AddTransient<ScreenSimulator.ViewModels.Ankang.ExitScreenViewModel>();
                services.AddTransient<ScreenSimulator.Views.Ankang.ExitScreenView>();
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
