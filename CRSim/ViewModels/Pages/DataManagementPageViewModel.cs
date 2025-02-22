namespace CRSim.ViewModels
{
    public partial class DataManagementPageViewModel(INavigationService navigationService) : ObservableObject
    {
        [ObservableProperty]
        private string _pageTitle = "数据管理";

        [ObservableProperty]
        private string _pageDescription = "车站、车次、交路等信息管理";

        [ObservableProperty]
        private ICollection<ControlInfoDataItem> _navigationCards = ControlsInfoDataSource.Instance.GetControlsInfo("Data Management");

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
