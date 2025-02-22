using CRSim.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CRSim.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private Settings _settings;
        private readonly JsonSerializerOptions options = new() { WriteIndented = true };
        public SettingsService(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
            LoadSettings();
        }
        public void SaveSettings()
        {
            string jsonString = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(_settingsFilePath, jsonString);
        }
        private void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                string jsonString = File.ReadAllText(_settingsFilePath);
                _settings = JsonSerializer.Deserialize<Settings>(jsonString);
            }
            else
            {
                _settings = new Settings();
            }
        }

        public Settings GetSettings()
        {
            return _settings;
        }
    }
}
