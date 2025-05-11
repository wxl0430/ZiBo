using CRSim.ScreenSimulator.ViewModels.Guangyuan;
namespace CRSim.ScreenSimulator.Views.Guangyuan
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
