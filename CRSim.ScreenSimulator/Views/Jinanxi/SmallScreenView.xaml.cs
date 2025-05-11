using CRSim.ScreenSimulator.ViewModels.Jinanxi;

namespace CRSim.ScreenSimulator.Views.Jinanxi
{
    public partial class SmallScreenView : BaseScreenView
    {
        public SmallScreenViewModel ViewModel { get; }
        public SmallScreenView(SmallScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
