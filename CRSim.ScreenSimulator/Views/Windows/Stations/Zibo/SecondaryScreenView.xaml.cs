using CRSim.ScreenSimulator.ViewModels.Zibo;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views.Zibo
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class SecondaryScreenView : Window
    {
        public SecondaryScreenViewModel ViewModel { get; }
        public SecondaryScreenView(SecondaryScreenViewModel viewModel)
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
