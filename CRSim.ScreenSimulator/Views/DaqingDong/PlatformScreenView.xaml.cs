using CRSim.ScreenSimulator.ViewModels.DaqingDong;

namespace CRSim.ScreenSimulator.Views.DaqingDong
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
