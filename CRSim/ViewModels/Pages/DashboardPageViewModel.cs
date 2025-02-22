namespace CRSim.ViewModels
{
    public partial class DashboardPageViewModel(INavigationService navigationService) : ObservableObject
    {
        [ObservableProperty]
        private ICollection<ControlInfoDataItem> _navigationCards = ControlsInfoDataSource.Instance.GetGroupedControlsInfo();

        private readonly INavigationService _navigationService = navigationService;

        [RelayCommand]
        public void Navigate(object pageType){
            if (pageType is Type page)
            {
                _navigationService.NavigateTo(page);
            }
        }

        
    }
}
