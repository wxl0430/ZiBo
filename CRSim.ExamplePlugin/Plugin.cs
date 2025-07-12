using CRSim.Core.Abstractions;
using CRSim.Core.Attributes;
using CRSim.ExamplePlugin.ViewModels;
using CRSim.ExamplePlugin.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CRSim.ExamplePlugin;

[PluginEntrance]
public class Plugin : PluginBase
{
    public Type ViewModel => typeof(ScreenViewModel);
    public Type View => typeof(ScreenView);
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddTransient(ViewModel);
        services.AddTransient(View);
    }
}