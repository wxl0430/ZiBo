namespace CRSim.Views
{
    /// <summary>
    /// DashboardPage.xaml 的交互逻辑
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage(DashboardPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }

        public DashboardPageViewModel ViewModel { get; }
    }
}
