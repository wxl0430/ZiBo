using CRSim.ScreenSimulator.ViewModels.ChengduDong;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace CRSim.ScreenSimulator.Views.ChengduDong
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PrimaryTicketCheckScreenView : BaseScreenView
    {
        public PrimaryTicketCheckScreenViewModel ViewModel { get; }
        public PrimaryTicketCheckScreenView(PrimaryTicketCheckScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
