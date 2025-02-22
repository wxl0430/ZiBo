using CRSim.ViewModels.ChengduDong;

namespace CRSim.Views.ChengduDong
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PrimaryTicketCheckScreenView : Window
    {
        public PlatformScreenViewModel ViewModel { get; }
        public PrimaryTicketCheckScreenView(PlatformScreenViewModel viewModel)
        {
            InitializeComponent();
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
