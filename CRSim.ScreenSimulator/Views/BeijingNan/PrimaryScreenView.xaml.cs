using CRSim.ScreenSimulator.ViewModels.BeijingNan;
namespace CRSim.ScreenSimulator.Views.BeijingNan
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
