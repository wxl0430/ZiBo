namespace CRSim.Services
{
    public interface INavigationService
    {
        void Navigate(Type type);

        void NavigateTo(Type type);

        void SetFrame(Frame frame);

        void NavigateBack();

        void NavigateForward();

        bool IsBackHistoryNonEmpty();

        event EventHandler<NavigatingEventArgs> Navigating;
    }
}
