using CRSim.Core.Models;

namespace CRSim.Views
{
    /// <summary>
    /// UserInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PlatformDialog : Window
    {
        public List<Platform> GeneratedPlatforms = [];
        public PlatformDialog()
        {
            WindowChrome.SetWindowChrome(
                this,
                new WindowChrome
                {
                    CaptionHeight = 50,
                    CornerRadius = default,
                    GlassFrameThickness = new Thickness(-1),
                    ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                    UseAeroCaptionButtons = true
                }
            );
            InitializeComponent();
            NameTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string[] platforms = NameTextBox.Text.Split('-');
            var Length = int.Parse(LengthTextBox.Text);
            if (platforms.Length == 2 && int.TryParse(platforms[0], out int startindex) && int.TryParse(platforms[1], out int endindex))
            {
                var startIndex = Math.Min(startindex, endindex);
                var endIndex = Math.Max(startindex, endindex);
                for (int i = startIndex; i < endIndex + 1; i++)
                {
                    GeneratedPlatforms.Add(new Platform { Name = i.ToString(), Length = Length });
                }
            }
            else
            {
                GeneratedPlatforms.Add(new Platform { Name = platforms[0], Length = Length });
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AccentButton == null) return;
            AccentButton.IsEnabled = !string.IsNullOrWhiteSpace(NameTextBox.Text) && !string.IsNullOrWhiteSpace(LengthTextBox.Text) && int.TryParse(LengthTextBox.Text, out int i) && i > 0;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter &&
                !string.IsNullOrWhiteSpace(NameTextBox.Text) &&
                !string.IsNullOrWhiteSpace(LengthTextBox.Text) && int.TryParse(LengthTextBox.Text, out int i) && i > 0)
            {
                OkButton_Click(null, null);
            }
            return;
        }
    }
}
