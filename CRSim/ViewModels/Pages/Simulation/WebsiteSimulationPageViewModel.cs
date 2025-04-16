using CRSim.WebsiteSimulator;

namespace CRSim.ViewModels;

public partial class WebsiteSimulationPageViewModel(Simulator simulator, IDialogService dialogService) : ObservableObject 
{
	[ObservableProperty]
	private string _pageTitle = "12306模拟";

	[ObservableProperty]
	private string _pageDescription = "";

    private readonly Simulator _simulator = simulator;

    private readonly IDialogService _dialogService = dialogService;

    [RelayCommand]
    public async Task StartSimulation()
    {
        try
        {
            await _simulator.Start();
        }
        catch(Exception e)
        {
            _dialogService.ShowMessage("认证失败", e.Message);
        }
    }
    [RelayCommand]
    public void StopSimulation()
    {
        _simulator.Stop();
    }
}
