using System.Diagnostics;

namespace CRSim.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPageViewModel ViewModel { get; } = App.AppHost.Services.GetService<SettingsPageViewModel>();

    public SettingsPage()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }
    private void Open_Issues(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/denglihong2007/CRSim/issues/new") { UseShellExecute = true });
    }
}
