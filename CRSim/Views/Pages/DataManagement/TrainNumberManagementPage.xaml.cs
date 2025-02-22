
namespace CRSim.Views;
public partial class TrainNumberManagementPage : Page
{
    public TrainNumberManagementPageViewModel ViewModel { get; }

    public TrainNumberManagementPage(TrainNumberManagementPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private void NumbersList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent,
            Source = sender
        };
        NumbersList.RaiseEvent(eventArg);
    }
}
