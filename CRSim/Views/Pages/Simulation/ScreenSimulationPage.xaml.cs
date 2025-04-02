namespace CRSim.Views;
    public partial class ScreenSimulationPage : Page
    {
        public ScreenSimulationPageViewModel ViewModel { get; }

        public ScreenSimulationPage(ScreenSimulationPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
