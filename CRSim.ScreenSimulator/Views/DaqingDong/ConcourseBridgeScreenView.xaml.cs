using CRSim.ScreenSimulator.ViewModels.DaqingDong;
namespace CRSim.ScreenSimulator.Views.DaqingDong
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class ConcourseBridgeScreenView : BaseScreenView
    {
        public ConcourseBridgeScreenViewModel ViewModel { get; }
        public ConcourseBridgeScreenView(ConcourseBridgeScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
