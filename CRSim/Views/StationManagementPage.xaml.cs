namespace CRSim.Views;

public sealed partial class StationManagementPage : Page
{
    public StationManagementPageViewModel ViewModel { get; } = App.AppHost.Services.GetService<StationManagementPageViewModel>();

    public StationManagementPage()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }
}
