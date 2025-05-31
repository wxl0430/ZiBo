using CRSim.ScreenSimulator.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CRSim.ScreenSimulator.Views
{
    public abstract class BaseScreenView : Window
    {
        public Guid SessionID;
        protected BaseScreenView()
        {
            Title = "引导屏";
            Background = Brushes.Transparent;
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            MouseDown += Window_MouseDown;
            KeyDown += Window_KeyDown;
            Unloaded += Window_Unloaded;
            SizeToContent = SizeToContent.WidthAndHeight;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            await DeleteScreenByGuidAsync();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private async Task DeleteScreenByGuidAsync()
        {
            if (File.Exists("Screen.json"))
            {
                var fileContent = await File.ReadAllTextAsync("Screen.json");
                var screenInfos = JsonSerializer.Deserialize<List<ScreenInfo>>(fileContent) ?? [];

                screenInfos.RemoveAll(screen => screen.SessionID == SessionID);

                var serializedContent = JsonSerializer.Serialize(screenInfos, CachedJsonSerializerOptions);
                await File.WriteAllTextAsync("Screen.json", serializedContent);
            }
        }
    }
}