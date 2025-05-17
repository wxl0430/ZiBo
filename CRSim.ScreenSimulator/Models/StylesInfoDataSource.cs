using System.Reflection;
using System.Text.Json;

namespace CRSim.ScreenSimulator.Models
{
    public class StylesInfoDataItem
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Region { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public Type ViewType
        {
            get
            {
                string type = $"CRSim.ScreenSimulator.Views.{UniqueId}View";
                return Assembly.Load("CRSim.ScreenSimulator").GetType(type);
            }
        }

        public Uri ImageSource 
        {
            get
            {
                return new Uri($"pack://application:,,,/CRSim.ScreenSimulator;component/Assets/{UniqueId.Replace('.', '/')}.png");
            }
        }

        public List<string> Parameters { get; set; } = [];

        public override string ToString()
        {
            return Title;
        }
    }

    public sealed class StylesInfoDataSource
    {

        #region Singleton

        private static readonly StylesInfoDataSource _instance;

        public static StylesInfoDataSource Instance
        {
            get
            {
                return _instance;
            }
        }

        static StylesInfoDataSource()
        {
            _instance = new StylesInfoDataSource();
        }

        private StylesInfoDataSource()
        {
            var jsonText = ReadStylesData();
            StylesInfo = JsonSerializer.Deserialize<List<StylesInfoDataItem>>(jsonText);
        }

        #endregion

        public ICollection<StylesInfoDataItem> StylesInfo { get; }

        private static string ReadStylesData()
        {
            var assembly = typeof(StylesInfoDataSource).Assembly;
            var resourceName = "CRSim.ScreenSimulator.Models.StylesInfoData.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new System.IO.StreamReader(stream);
            return reader.ReadToEnd();
        }

        public ICollection<string> GetParametersInfo(Type type)
        {
            return StylesInfo.FirstOrDefault(x => x.ViewType == type)?.Parameters;
        }

        public string GetStyleName(Type type)
        {
            return StylesInfo.FirstOrDefault(x => x.ViewType == type)?.Title;
        }
    }
}
