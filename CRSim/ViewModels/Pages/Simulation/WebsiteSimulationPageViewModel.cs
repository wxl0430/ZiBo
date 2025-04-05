using CRSim.WebsiteSimulator;

namespace CRSim.ViewModels;

public partial class WebsiteSimulationPageViewModel : ObservableObject 
{
	[ObservableProperty]
	private string _pageTitle = "12306模拟";

	[ObservableProperty]
	private string _pageDescription = "";

    private Simulator _simulator;

    public WebsiteSimulationPageViewModel(Simulator simulator)
    {
        _simulator = simulator;
    }
    [RelayCommand]
    public async Task StartSimulation()
    {
        await _simulator.Start();
    }
    [RelayCommand]
    public void StopSimulation()
    {
        _simulator.Stop();
    }
}
