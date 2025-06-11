using CRSim.ScreenSimulator.ViewModels.DezhouDong;

namespace CRSim.ScreenSimulator.Views.DezhouDong
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
