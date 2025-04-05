
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

    private void TrainNumbersList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent,
            Source = sender
        };
        TrainNumbersList.RaiseEvent(eventArg);
    }

    private void TimeTableList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent,
            Source = sender
        };
        TimeTableList.RaiseEvent(eventArg);
    }

    private void TicketsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent,
            Source = sender
        };
        TicketsList.RaiseEvent(eventArg);
    }

    private void PricesList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent,
            Source = sender
        };
        PricesList.RaiseEvent(eventArg);
    }
}
