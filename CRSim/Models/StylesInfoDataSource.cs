namespace CRSim.Models
{
    public class StylesInfoDataItem
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Station { get; set; }
        public string StyleName { get; set; }
        public string ImagePath { get; set; }
        public Type ViewType
        {
            get
            {
                string type = $"CRSim.ScreenSimulator.Views.{Station}.{StyleName}";
                return Assembly.Load("CRSim.ScreenSimulator").GetType(type);
            }
        }

        public Uri ImageSource 
        {
            get
            {
                return new Uri($"pack://application:,,,/{ImagePath}");
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
            var resourceName = "CRSim.Models.StylesInfoData.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new System.IO.StreamReader(stream);
            return reader.ReadToEnd();
        }

        public ICollection<string> GetParametersInfo(Type type)
        {
            return StylesInfo.Where(x => x.ViewType == type).FirstOrDefault()?.Parameters;
        }

        public string GetStyleName(Type type)
        {
            return StylesInfo.Where(x => x.ViewType == type).FirstOrDefault()?.Title;
        }
    }
}
