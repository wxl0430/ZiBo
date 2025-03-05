using CRSim.Core.Models;
using Microsoft.Win32;
using System.Text.Json;

namespace CRSim.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private Settings _settings;
        private RegistryKey _key = Registry.CurrentUser.OpenSubKey(@"Software\CRSim\Settings",true);
        public SettingsService()
        {
            LoadSettings();
        }
        public void SaveSettings()
        {
            _key.SetValue("TimeOffset", (int)_settings.TimeOffset.TotalMinutes);
            _key.SetValue("SwitchPageSeconds", _settings.SwitchPageSeconds);
            _key.SetValue("MaxPages", _settings.MaxPages);
            _key.SetValue("StopCheckInAdvanceDuration", (int)_settings.StopCheckInAdvanceDuration.TotalMinutes);
            _key.SetValue("StopDisplayUntilDepartureDuration", (int)_settings.StopDisplayUntilDepartureDuration.TotalMinutes);
            _key.SetValue("PassingCheckInAdvanceDuration", (int)_settings.PassingCheckInAdvanceDuration.TotalMinutes);
            _key.SetValue("DepartureCheckInAdvanceDuration", (int)_settings.DepartureCheckInAdvanceDuration.TotalMinutes);
        }
        private void LoadSettings()
        {
            if (_key==null)
            {
                Registry.CurrentUser.CreateSubKey(@"Software\CRSim\Settings");
                _key = Registry.CurrentUser.OpenSubKey(@"Software\CRSim\Settings",true);
                _settings = new Settings();
                SaveSettings();
            }
            else
            {
                _settings = new Settings();
                _settings.TimeOffset = TimeSpan.FromMinutes((int)_key.GetValue("TimeOffset"));
                _settings.SwitchPageSeconds = (int)_key.GetValue("SwitchPageSeconds");
                _settings.MaxPages = (int)_key.GetValue("MaxPages");
                _settings.StopCheckInAdvanceDuration = TimeSpan.FromMinutes((int)_key.GetValue("StopCheckInAdvanceDuration"));
                _settings.StopDisplayUntilDepartureDuration = TimeSpan.FromMinutes((int)_key.GetValue("StopDisplayUntilDepartureDuration"));
                _settings.PassingCheckInAdvanceDuration = TimeSpan.FromMinutes((int)_key.GetValue("PassingCheckInAdvanceDuration"));
                _settings.DepartureCheckInAdvanceDuration = TimeSpan.FromMinutes((int)_key.GetValue("DepartureCheckInAdvanceDuration"));
            }
        }

        public Settings GetSettings()
        {
            return _settings;
        }
    }
}
