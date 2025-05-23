using CRSim.ScreenSimulator.ViewModels.BeijingNan;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace CRSim.ScreenSimulator.Views.BeijingNan
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
