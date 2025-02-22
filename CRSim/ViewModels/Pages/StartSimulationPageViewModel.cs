namespace CRSim.ViewModels
{
    public partial class StartSimulationPageViewModel(INavigationService navigationService) : ObservableObject
    {
        [ObservableProperty]
        private string _pageTitle = "开始模拟";

        [ObservableProperty]
        private string _pageDescription = "请选择你想模拟的项目";

        [ObservableProperty]
        private ICollection<ControlInfoDataItem> _navigationCards = ControlsInfoDataSource.Instance.GetControlsInfo("Start Simulation");

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
