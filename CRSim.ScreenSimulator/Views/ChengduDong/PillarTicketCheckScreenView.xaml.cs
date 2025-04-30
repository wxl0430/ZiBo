using CRSim.ScreenSimulator.ViewModels.ChengduDong;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views.ChengduDong
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PillarTicketCheckScreenView : Window
    {
        public PillarTicketCheckScreenViewModel ViewModel { get; }
        public PillarTicketCheckScreenView(PillarTicketCheckScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            DataContext = this;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
