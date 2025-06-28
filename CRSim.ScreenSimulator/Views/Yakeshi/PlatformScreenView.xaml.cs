using CRSim.ScreenSimulator.ViewModels.Yakeshi;

namespace CRSim.ScreenSimulator.Views.Yakeshi
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
