
namespace CRSim.Views;
public partial class TrainNumberManagementPage : Page
{
    public TrainNumberManagementPageViewModel ViewModel { get; }

    public TrainNumberManagementPage(TrainNumberManagementPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}
