using CRSim.ScreenSimulator.ViewModels.ChengduDong;

namespace CRSim.ScreenSimulator.Views.ChengduDong
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PillarTicketCheckScreenView : BaseScreenView
    {
        public PillarTicketCheckScreenViewModel ViewModel { get; }
        public PillarTicketCheckScreenView(PillarTicketCheckScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
