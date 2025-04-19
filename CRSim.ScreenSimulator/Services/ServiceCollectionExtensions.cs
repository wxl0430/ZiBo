using CRSim.ScreenSimulator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CRSim.ScreenSimulator.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScreenSimulatorServices(this IServiceCollection services)
        {
            foreach (var uniqueId in StylesInfoDataSource.Instance.StylesInfo.Select(x => x.UniqueId))
            {
                var viewModelType = Type.GetType($"CRSim.ScreenSimulator.ViewModels.{uniqueId}ViewModel");
                if (viewModelType != null)
                {
                    services.AddTransient(viewModelType);
                }

                var viewType = Type.GetType($"CRSim.ScreenSimulator.Views.{uniqueId}View");
                if (viewType != null)
                {
                    services.AddTransient(viewType);
                }
            }
            return services;
        }
    }
}
