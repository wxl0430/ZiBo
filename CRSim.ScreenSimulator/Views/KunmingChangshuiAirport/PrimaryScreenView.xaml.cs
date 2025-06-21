using CRSim.ScreenSimulator.ViewModels.KunmingChangshuiAirport;

namespace CRSim.ScreenSimulator.Views.KunmingChangshuiAirport
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
