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

    public ObservableCollection<PluginInfo> Plugins = IPluginService.OnlinePlugins;

    private readonly IPluginService _pluginService;
    private readonly IDialogService _dialogService;

    public PluginManagementPageViewModel(IPluginService pluginService,IDialogService dialogService)
    {
        _pluginService = pluginService;
        _dialogService = dialogService;
        _pluginService.LoadOnlinePluginsAsync();
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
    public async Task InstallPluginOnline()
    {
        await _pluginService.InstallPluginOnlineAsync(SelectedPlugin);
        OnPropertyChanged(nameof(SelectedPlugin));
    }
    [RelayCommand]
    public async Task InstallPluginLocal()
    {
        if (await _dialogService.GetFileAsync([".crsp"]) is string filePath)
        {
            try
            {
                await _pluginService.InstallPluginLocalAsync(filePath);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowTextAsync("安装失败", $"{ex}");
            }
        }
        else
        {
            await _dialogService.ShowMessageAsync("未选择文件", "请先选择一个插件包文件。");
        }
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
    public async Task PackPlugin()
    {
        if (SelectedPlugin == null) return;
        string? filePath = await _dialogService.SaveFileAsync(".crsp", $"{SelectedPlugin.Manifest.Id}");
        if (filePath == null) return;
        try
        {
            await _pluginService.PackPluginAsync(SelectedPlugin, filePath);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowTextAsync("打包失败", $"{ex}");
        }
    }
    [RelayCommand]
    public static void Restart()
    {
        string exePath = Environment.ProcessPath;
        Process.Start(new ProcessStartInfo(exePath)
        {
            UseShellExecute = true 
        });
        Application.Current.Exit();
    }

    [RelayCommand]
    public void SourceSelected(object args)
    {
        if(args is SelectorBarItem selectorBarItem)
        {
            SelectedPlugin = null;
            Plugins = (selectorBarItem?.Text == "本地") ? IPluginService.OnlinePlugins : IPluginService.LoadedPlugins;
            OnPropertyChanged(nameof(Plugins));
        }
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
    public async Task Refresh()
    {
        await _pluginService.LoadOnlinePluginsAsync();
    }
}