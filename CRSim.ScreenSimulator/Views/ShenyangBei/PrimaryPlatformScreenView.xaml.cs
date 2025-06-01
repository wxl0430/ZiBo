using CRSim.ScreenSimulator.ViewModels.ShenyangBei;

namespace CRSim.ScreenSimulator.Views.ShenyangBei
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
