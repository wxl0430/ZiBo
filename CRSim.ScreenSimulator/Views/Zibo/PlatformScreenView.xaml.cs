using CRSim.ScreenSimulator.ViewModels.Zibo;

namespace CRSim.ScreenSimulator.Views.Zibo
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
