using CRSim.ScreenSimulator.ViewModels.GuangdongIntercity;

namespace CRSim.ScreenSimulator.Views.GuangdongIntercity
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
