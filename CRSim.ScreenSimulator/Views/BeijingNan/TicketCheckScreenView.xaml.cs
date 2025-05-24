using CRSim.ScreenSimulator.ViewModels.BeijingNan;
namespace CRSim.ScreenSimulator.Views.BeijingNan
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
