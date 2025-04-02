using CRSim.ScreenSimulator.ViewModels.ChengduMetro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views.ChengduMetro
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
            ViewModel = viewModel;
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            DataContext = this;
            player.Source = new Uri("Assets\\Advertisement.mp4", UriKind.Relative);
            player.Play();
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

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }

    }
}
