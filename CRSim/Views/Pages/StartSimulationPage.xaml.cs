namespace CRSim.Views;

public partial class StartSimulationPage : Page
{
    public StartSimulationPageViewModel ViewModel { get; } 
		public StartSimulationPage(StartSimulationPageViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = this;
    }
}
