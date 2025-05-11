using CRSim.ScreenSimulator.ViewModels.Zibo;

namespace CRSim.ScreenSimulator.Views.Zibo
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
