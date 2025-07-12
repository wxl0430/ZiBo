namespace CRSim.Core.Utils
{
    public static class AppPaths
    {
        public static string AppDataFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CRSim");

        public static string ConfigFilePath =>
            Path.Combine(AppDataFolder, "data.json");

        public static string PluginsRootPath =>
            Path.Combine(AppDataFolder, "Plugins");
    }
}
