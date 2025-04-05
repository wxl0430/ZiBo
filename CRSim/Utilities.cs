namespace CRSim
{
    internal static class Utilities
    {
        public static bool IsBackdropSupported()
        {
            var os = Environment.OSVersion;
            var version = os.Version;

            return version.Major >= 10 && version.Build >= 22621;
        }

        public static bool IsBackdropDisabled()
        {
            var appContextBackdropData = AppContext.GetData("Switch.System.Windows.Appearance.DisableFluentThemeWindowBackdrop");
            bool disableFluentThemeWindowBackdrop = false;

            if (appContextBackdropData != null)
            {
                disableFluentThemeWindowBackdrop = bool.Parse(Convert.ToString(appContextBackdropData));
            }

            return disableFluentThemeWindowBackdrop;
        }
        public static TimeSpan RoundToMinute(string timeString)
        {
            if (TimeSpan.TryParse(timeString, out var time))
                return TimeSpan.FromMinutes(Math.Round(time.TotalMinutes));
            return default;
        }
    }

}
