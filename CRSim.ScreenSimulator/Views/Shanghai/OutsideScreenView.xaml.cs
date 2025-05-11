using CRSim.ScreenSimulator.ViewModels.Shanghai;

namespace CRSim.ScreenSimulator.Views.Shanghai
{
    public partial class OutsideScreenView : BaseScreenView
    {
        public OutsideScreenViewModel ViewModel { get; }
        public OutsideScreenView(OutsideScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
