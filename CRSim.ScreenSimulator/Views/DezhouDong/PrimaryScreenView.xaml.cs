using CRSim.ScreenSimulator.ViewModels.DezhouDong;
namespace CRSim.ScreenSimulator.Views.DezhouDong
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
