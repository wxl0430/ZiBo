using CRSim.ScreenSimulator.ViewModels.Harbin;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views.Harbin
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PrimaryScreenView : Window
    {
        public PrimaryScreenViewModel ViewModel { get; }
        public PrimaryScreenView(PrimaryScreenViewModel viewModel)
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
