namespace CRSim.Views;
    public partial class WebsiteSimulationPage : Page
    {
        public WebsiteSimulationPageViewModel ViewModel { get; }

        public WebsiteSimulationPage(WebsiteSimulationPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
