namespace CRSim.Views;

public partial class DataManagementPage : Page
{
    public DataManagementPageViewModel ViewModel { get; } 
		public DataManagementPage(DataManagementPageViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = this;
    }
}
