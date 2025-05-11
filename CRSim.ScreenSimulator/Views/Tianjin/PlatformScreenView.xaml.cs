using CRSim.ScreenSimulator.ViewModels.Tianjin;

namespace CRSim.ScreenSimulator.Views.Tianjin
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
