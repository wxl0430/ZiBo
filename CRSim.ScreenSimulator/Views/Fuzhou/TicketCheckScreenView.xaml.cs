using CRSim.ScreenSimulator.ViewModels.Fuzhou;

namespace CRSim.ScreenSimulator.Views.Fuzhou
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
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
