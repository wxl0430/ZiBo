using CRSim.ScreenSimulator.ViewModels.XuzhouMetro;
using System.Windows;

namespace CRSim.ScreenSimulator.Views.XuzhouMetro
{
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
