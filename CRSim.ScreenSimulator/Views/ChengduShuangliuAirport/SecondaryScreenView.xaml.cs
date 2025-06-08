using CRSim.ScreenSimulator.ViewModels.ChengduShuangliuAirport;

namespace CRSim.ScreenSimulator.Views.ChengduShuangliuAirport
{
    public partial class SecondaryScreenView : BaseScreenView
    {
        public SecondaryScreenViewModel ViewModel { get; }
        public SecondaryScreenView(SecondaryScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
