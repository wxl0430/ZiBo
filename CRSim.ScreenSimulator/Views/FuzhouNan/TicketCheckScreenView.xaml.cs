using CRSim.ScreenSimulator.ViewModels.FuzhouNan;

namespace CRSim.ScreenSimulator.Views.FuzhouNan
{
    public partial class TicketCheckScreenView : BaseScreenView
    {
        public TicketCheckScreenViewModel ViewModel { get; }
        public TicketCheckScreenView(TicketCheckScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
