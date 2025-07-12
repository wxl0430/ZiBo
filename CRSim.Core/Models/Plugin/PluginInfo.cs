using CommunityToolkit.Mvvm.ComponentModel;
using CRSim.Core.Enums;

namespace CRSim.Core.Models.Plugin;
public partial class PluginInfo : ObservableRecipient
{
    //private DownloadProgress? _downloadProgress;
    //private bool _isAvailableOnMarket = false;
    [ObservableProperty]
    private PluginManifest _manifest = new();
    [ObservableProperty]
    private bool _restartRequired = false;
    public PluginLoadStatus LoadStatus { get; internal set; } = PluginLoadStatus.NotLoaded;
    public bool IsEnabled
    {
        get
        {
            return !File.Exists(Path.Combine(PluginFolderPath, ".disabled"));
        }
        set
        {
            var path = Path.Combine(PluginFolderPath, ".disabled");
            RestartRequired = true;
            if (value)
            {
                File.Delete(path);
            }
            else
            {
                File.WriteAllText(path, "");
            }
            OnPropertyChanged();
        }
    }
    public string PluginFolderPath { get; internal set; } = "";
    public string RealIconPath { get; set; } = "";
    public bool IsUninstalling
    {
        get
        {
            return File.Exists(Path.Combine(PluginFolderPath, ".uninstall"));
        }
        set
        {
            var path = Path.Combine(PluginFolderPath, ".uninstall");
            if (value)
            {
                File.WriteAllText(path, "");
            }
            else
            {
                File.Delete(path);
            }
            OnPropertyChanged();
        }
    }
    public Exception? Exception { get; internal set; }
    //private bool _isUpdateAvailable = false;

    [ObservableProperty]
    private StyleInfo? styleInfo;
}