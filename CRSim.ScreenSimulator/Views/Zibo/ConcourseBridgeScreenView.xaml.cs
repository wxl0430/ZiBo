using CRSim.ScreenSimulator.ViewModels.Zibo;
namespace CRSim.ScreenSimulator.Views.Zibo
{
    public partial class ConcourseBridgeScreenView : BaseScreenView
    {
        public ConcourseBridgeScreenViewModel ViewModel { get; }
        public ConcourseBridgeScreenView(ConcourseBridgeScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
