
using CRSim.ExamplePlugin.ViewModels;
using System.Windows.Controls;
namespace CRSim.ExamplePlugin.Views
{
    public partial class ScreenView : Page
    {
        public ScreenViewModel ViewModel { get; }
        public ScreenView(ScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
