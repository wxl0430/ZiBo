using CRSim.ScreenSimulator.ViewModels.Niuzhou;

namespace CRSim.ScreenSimulator.Views.Niuzhou
{
    public partial class PrimaryScreenView : BaseScreenView
    {
        public PrimaryScreenViewModel ViewModel { get; }
        public PrimaryScreenView(PrimaryScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
