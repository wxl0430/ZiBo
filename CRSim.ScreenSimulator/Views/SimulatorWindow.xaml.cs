using CRSim.Core.Abstractions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views
{
    public partial class SimulatorWindow : Window
    {
        public SimulatorWindow(Page page)
        {
            InitializeComponent();
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            contentFrame.Content = page;
        }
    }
}
