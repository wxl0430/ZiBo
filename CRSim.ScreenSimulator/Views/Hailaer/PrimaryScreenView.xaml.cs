using CRSim.ScreenSimulator.ViewModels.Hailaer;
namespace CRSim.ScreenSimulator.Views.Hailaer
{
    public partial class PrimaryScreenView : BaseScreenView
    {
        public PrimaryScreenViewModel ViewModel { get; }
        public PrimaryScreenView(PrimaryScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
