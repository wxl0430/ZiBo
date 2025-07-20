using CRSim.Core.Abstractions;
using CRSim.Core.Models;
using CRSim.Core.Models.Plugin;
using CRSim.Core.Services;
using System.Diagnostics;

namespace CRSim.ViewModels;

public partial class PluginManagementPageViewModel : ObservableObject
{

    [ObservableProperty]
    public partial string PageTitle { get; set; } = "插件管理";

    [ObservableProperty]
    public partial PluginInfo? SelectedPlugin { get; set; }

    public ObservableCollection<PluginInfo> Plugins = [];

    private IPluginService _pluginService;

    public PluginManagementPageViewModel(IPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    [RelayCommand]
    public void PluginSelected(object args)
    {
        if(args is SelectionChangedEventArgs selectionChangedEventArgs && selectionChangedEventArgs.AddedItems.Count == 1)
        {
            SelectedPlugin = (PluginInfo)selectionChangedEventArgs.AddedItems.FirstOrDefault();
        }
    }

    [RelayCommand]
    public async Task InstallPlugin()
    {
        await _pluginService.InstallPluginAsync(SelectedPlugin);
        OnPropertyChanged(nameof(SelectedPlugin));
    }

    [RelayCommand]
    public void UninstallPlugin()
    {
        SelectedPlugin.IsUninstalling = true;
        OnPropertyChanged(nameof(SelectedPlugin));
    }

    [RelayCommand]
    public void CancelUninstallPlugin()
    {
        SelectedPlugin.IsUninstalling = false;
        OnPropertyChanged(nameof(SelectedPlugin));
    }

    [RelayCommand]
    public static void Restart()
    {
        string exePath = Process.GetCurrentProcess().MainModule.FileName;
        Process.Start(new ProcessStartInfo(exePath)
        {
            UseShellExecute = true 
        });
        Application.Current.Exit();
    }

    [RelayCommand]
    public void SourceSelected(SelectorBarItem selectorBarItem)
    {
        SelectedPlugin = null;
        Plugins = (selectorBarItem?.Text == "本地" || selectorBarItem is null)
            ? IPluginService.OnlinePlugins : IPluginService.LoadedPlugins;
        OnPropertyChanged(nameof(Plugins));
    }

    [RelayCommand]
    public static void OpenPluginFolder()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = AppPaths.PluginsRootPath,
            UseShellExecute = true
        });
    }

    [RelayCommand]
    public void Refresh()
    {
        _pluginService.LoadOnlinePlugins();
    }
}