using CRSim.ScreenSimulator.ViewModels.Tianjin;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views.Tianjin
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PlatformScreenView : Window
    {
        public PlatformScreenViewModel ViewModel { get; }
        public PlatformScreenView(PlatformScreenViewModel viewModel)
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
