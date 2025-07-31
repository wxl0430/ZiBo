using System.Diagnostics;

namespace CRSim.Views;

public sealed partial class PluginManagementPage : Page
{
    public PluginManagementPageViewModel ViewModel { get; } = App.AppHost.Services.GetService<PluginManagementPageViewModel>();

    public PluginManagementPage()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }

    private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
    }
}
