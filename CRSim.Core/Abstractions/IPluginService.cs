using CRSim.Core.Models.Plugin;
using System.Collections.ObjectModel;

namespace CRSim.Core.Abstractions;

/// <summary>
/// 插件服务。用于管理应用各插件的加载和设置。
/// </summary>
public interface IPluginService
{
    /// <summary>
    /// 插件包文件扩展名。
    /// </summary>
    public static readonly string PluginPackageExtension = ".cp";

    internal static ObservableCollection<PluginInfo> LoadedPluginsInternal { get; } = [];

    /// <summary>
    /// 已加载的插件信息列表。
    /// </summary>
    public static IReadOnlyList<PluginInfo> LoadedPlugins => LoadedPluginsInternal;
}