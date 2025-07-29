using CRSim.Core.Abstractions;
using CRSim.Core.Enums;
using CRSim.Core.Models;
using CRSim.Core.Models.Plugin;
using CRSim.ScreenSimulator.Views;
using System.Windows;
using System.Windows.Controls;
namespace CRSim.ScreenSimulator
{
	public class StyleManager
	{
        public static List<PluginInfo> StyleInfos => [.. IPluginService.LoadedPlugins.Where(x => x.Manifest.Type == "ScreenStyle" && x.LoadStatus == PluginLoadStatus.Loaded)];
        public static IServiceProvider ServiceProvider;
        private static IDatabaseService _databaseService;
        public StyleManager(IEnumerable<PluginBase> pluginBases,IServiceProvider serviceProvider,IDatabaseService databaseService)
		{
            ServiceProvider = serviceProvider;
            _databaseService = databaseService;
        }
        public static void ShowWindow(Type page, Station station, string ticketCheck, string platform,string text,int location,string video)
        {
            Thread thread = new(() =>
            {
                Page viewInstance = (Page)ServiceProvider.GetService(page);
                var viewModel = ((dynamic)viewInstance).ViewModel;
                viewModel.UIDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
                if (text != null)
                {
                    viewModel.Text = text;
                }
                if (location != 0)
                {
                    viewModel.Location = location;
                }
                if (video != null)
                {
                    viewModel.Video = video;
                }
                viewModel.LoadData(station, ticketCheck, platform);
                var win = new SimulatorWindow(viewInstance);
                win.Show();
                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA); 
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
