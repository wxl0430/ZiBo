using CRSim.Core.Abstractions;
using CRSim.ScreenSimulator.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views
{
    public partial class SimulatorWindow : Window
    {
        private readonly ITimeService _timeService;

        public SimulatorWindow(Page page,ITimeService timeService)
        {
            InitializeComponent();
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            contentFrame.Content = page;
            _timeService = timeService;
            foreach (var resource in page.Resources.Values)
            {
                if (resource is IHasTimeService needsTime)
                {
                    needsTime.TimeService = timeService;
                }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void DecreaseSpeed(object sender, MouseButtonEventArgs e)
        {
            _timeService.Speed /= 2 ;
            SpeedText.Text = _timeService.Speed.ToString()+"x" ;
        }
        private void IncreaseSpeed(object sender, MouseButtonEventArgs e)
        {
            _timeService.Speed *= 2;
            SpeedText.Text = _timeService.Speed.ToString() + "x";
        }

        private void Close(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
