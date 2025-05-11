using CRSim.ScreenSimulator.ViewModels.HarbinXi;

namespace CRSim.ScreenSimulator.Views.HarbinXi
{
    public partial class PlatformScreenView : BaseScreenView
    {
        public PlatformScreenViewModel ViewModel { get; }
        public PlatformScreenView(PlatformScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
