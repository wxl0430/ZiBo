using CRSim.ScreenSimulator.ViewModels.Guangyuan;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace CRSim.ScreenSimulator.Views.Guangyuan
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PrimaryScreenView : Window
    {
        public OutsideScreenViewModel ViewModel { get; }
        public PrimaryScreenView(OutsideScreenViewModel viewModel)
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
