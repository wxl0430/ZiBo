using CRSim.ScreenSimulator.ViewModels.BeijingNan;

namespace CRSim.ScreenSimulator.Views.BeijingNan
{
    public partial class SecondaryPlatformScreenView : BaseScreenView
    {
        public SecondaryPlatformScreenViewModel ViewModel { get; }
        public SecondaryPlatformScreenView(SecondaryPlatformScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
