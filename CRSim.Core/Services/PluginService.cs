using CRSim.Core.Abstractions;
using CRSim.Core.Attributes;
using CRSim.Core.Enums;
using CRSim.Core.Models;
using CRSim.Core.Models.Plugin;
using CRSim.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CRSim.Core.Services;

public class PluginService : IPluginService
{
    public static readonly string PluginManifestFileName = "manifest.yml"; 

    public static void InitializePlugins(HostBuilderContext context, IServiceCollection services,string externalPluginPath)
    {
        if(!Directory.Exists(AppPaths.PluginsRootPath))
        {
            Directory.CreateDirectory(AppPaths.PluginsRootPath);
        }
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var pluginDirs = Directory.EnumerateDirectories(AppPaths.PluginsRootPath);
        if(!string.IsNullOrEmpty(externalPluginPath))
        {
            pluginDirs = pluginDirs.Append(externalPluginPath);
        }

        foreach (var pluginDir in pluginDirs)
        {
            if (string.IsNullOrWhiteSpace(pluginDir))
                continue;
            var manifestPath = Path.Combine(pluginDir, PluginManifestFileName);
            if (!File.Exists(manifestPath))
            {
                continue;
            }

            var manifestYaml = File.ReadAllText(manifestPath);
            var manifest = deserializer.Deserialize<PluginManifest?>(manifestYaml);
            if (manifest == null)
            {
                continue;
            }
            var info = new PluginInfo
            {
                Manifest = manifest,
                PluginFolderPath = Path.GetFullPath(pluginDir),
                RealIconPath = Path.Combine(Path.GetFullPath(pluginDir), manifest.Icon),
                StyleInfo = manifest.Type == "ScreenStyle" ? deserializer.Deserialize<StyleInfo?>(File.ReadAllText(Path.Combine(Path.GetFullPath(pluginDir), "style.yml"))) : null,
            };
            if (info.IsUninstalling)
            {
                Directory.Delete(pluginDir, true);
                continue;
            }
            IPluginService.LoadedPluginsInternal.Add(info);
            if (!info.IsEnabled)
            {
                info.LoadStatus = PluginLoadStatus.Disabled;
            }
        }

        foreach (var info in IPluginService.LoadedPluginsInternal)
        {
            var manifest = info.Manifest;
            var pluginDir = info.PluginFolderPath;
            try
            {
                var fullPath = Path.GetFullPath(Path.Combine(pluginDir, manifest.EntranceAssembly));
                var asm = Assembly.LoadFrom(fullPath);
                var entrance = asm.ExportedTypes.FirstOrDefault(x =>
                    x.BaseType == typeof(PluginBase) ||
                    x.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(PluginEntrance)) != null);

                if (entrance == null)
                {
                    continue;
                }

                if (Activator.CreateInstance(entrance) is not PluginBase entranceObj)
                {
                    continue;
                }

                entranceObj.Info = info;
                entranceObj.Initialize(context, services);
                services.AddSingleton(typeof(PluginBase), entranceObj);
                services.AddSingleton(entrance, entranceObj);
                info.LoadStatus = PluginLoadStatus.Loaded;
                Console.WriteLine($"Initialize plugin: {pluginDir} ({manifest.Version})");
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.LoadStatus = PluginLoadStatus.Error;
            }
        }        
    }

}