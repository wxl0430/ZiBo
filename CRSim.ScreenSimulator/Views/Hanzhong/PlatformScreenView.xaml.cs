using CRSim.ScreenSimulator.ViewModels.Hanzhong;

namespace CRSim.ScreenSimulator.Views.Hanzhong
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
