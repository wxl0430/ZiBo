using CRSim.ScreenSimulator.ViewModels.BeijingNan;

namespace CRSim.ScreenSimulator.Views.BeijingNan
{
    public partial class PrimaryPlatformScreenView : BaseScreenView
    {
        public PrimaryPlatformScreenViewModel ViewModel { get; }
        public PrimaryPlatformScreenView(PrimaryPlatformScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
