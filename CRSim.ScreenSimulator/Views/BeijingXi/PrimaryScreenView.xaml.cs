using CRSim.ScreenSimulator.ViewModels.BeijingXi;
namespace CRSim.ScreenSimulator.Views.BeijingXi
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
