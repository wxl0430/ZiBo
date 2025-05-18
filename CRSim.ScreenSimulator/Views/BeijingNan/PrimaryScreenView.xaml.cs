using CRSim.ScreenSimulator.ViewModels.BeijingNan;
using CRSim.ScreenSimulator.Converters;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views.BeijingNan
{
    public partial class PrimaryScreenView : BaseScreenView
    {
        public PrimaryScreenViewModel ViewModel { get; }
        public PrimaryScreenView(PrimaryScreenViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;

            var trainStateColorConverter = (TrainStateColorConverter)Resources["TrainStateColorConverter"];
            trainStateColorConverter.WaitingColorList = new List<SolidColorBrush> 
            { 
                new SolidColorBrush(Colors.White), 
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#edd16f")) 
            };
        }
    }
}
