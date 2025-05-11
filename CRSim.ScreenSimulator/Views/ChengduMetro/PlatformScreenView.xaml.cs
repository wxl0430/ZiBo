using CRSim.ScreenSimulator.ViewModels.ChengduMetro;
using System.Windows;

namespace CRSim.ScreenSimulator.Views.ChengduMetro
{
    /// <summary>
    /// SecondaryScreen.xaml 的交互逻辑
    /// </summary>
    public partial class PlatformScreenView : BaseScreenView
    {
        public PlatformScreenViewModel ViewModel { get; }
        public PlatformScreenView(PlatformScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
            player.Play();
        }
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }
    }
}
