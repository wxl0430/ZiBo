using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace CRSim.Core.Models.Plugin;

/// <summary>
/// 插件元数据
/// </summary>
public class PluginManifest
{
    /// <summary>
    /// 入口程序集。加载插件时，将在此入口程序集中搜索插件类。
    /// </summary>
    /// <example>MyPlugin.dll</example>
    [JsonPropertyName("entranceAssembly")]
    public string EntranceAssembly { get; set; } = "";

    /// <summary>
    /// 插件显示名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    /// <summary>
    /// 插件ID。
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    /// <summary>
    /// 插件自述。
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    /// <summary>
    /// 项目 Url
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 插件版本
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    /// <summary>
    /// 插件目标 CRSim 版本
    /// </summary>
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = "";

    /// <summary>
    /// 插件作者
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; set; } = "";

    /// <summary>
    /// 插件类型
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
}