namespace CRSim.Views;

public sealed partial class ScreenSimulatorPage : Page
{
    public ScreenSimulatorPageViewModel ViewModel { get; } = App.AppHost.Services.GetService<ScreenSimulatorPageViewModel>();

    public ScreenSimulatorPage()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var dialogService = App.AppHost.Services.GetService<IDialogService>()!;
        (dialogService as DialogService)?.SetXamlRoot(XamlRoot);
    }
}
