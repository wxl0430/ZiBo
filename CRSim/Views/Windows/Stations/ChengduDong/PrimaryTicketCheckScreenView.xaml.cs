using CRSim.ViewModels.ChengduDong;

namespace CRSim.Views.ChengduDong
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PrimaryTicketCheckScreenView : Window
    {
        public PrimaryTicketCheckScreenViewModel ViewModel { get; }
        public PrimaryTicketCheckScreenView(PrimaryTicketCheckScreenViewModel viewModel)
        {
            InitializeComponent();
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            ViewModel = viewModel;
            DataContext = viewModel;
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
