using CRSim.ScreenSimulator.ViewModels.Mianyang;

namespace CRSim.ScreenSimulator.Views.Mianyang
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
