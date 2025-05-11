using CRSim.ScreenSimulator.ViewModels.Beijing;
namespace CRSim.ScreenSimulator.Views.Beijing
{
    public partial class ArrivalScreenView : BaseScreenView
    {
        public ArrivalScreenViewModel ViewModel { get; }
        public ArrivalScreenView(ArrivalScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}