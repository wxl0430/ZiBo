using System.Text.RegularExpressions;

namespace CRSim.Views
{
    public partial class TicketCheckDialog : Window
    {
        public string WaitingAreaName;
        public List<string> TicketCheck;
        public TicketCheckDialog(List<string> waitingAreaNames)
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
            foreach (var n in waitingAreaNames)
            {
                WaitingAreasComboBox.Items.Add(n);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TicketCheck = Generate(TicketCheckInputBox.Text);
            WaitingAreaName = WaitingAreasComboBox.SelectedItem.ToString();
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
            AccentButton.IsEnabled = !string.IsNullOrWhiteSpace(TicketCheckInputBox.Text) && WaitingAreasComboBox.SelectedItem!=null;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && !string.IsNullOrWhiteSpace(TicketCheckInputBox.Text))
            {
                OkButton_Click(null, null);
            }
            return;
        }

        public static List<string> Generate(string input)
        {
            if (!input.Contains('-'))
            {
                return [input];
            }

            string[] parts = input.Split(['-'], 2);
            if (parts.Length != 2)
            {
                return [input];
            }

            var startMatch = Regex.Match(parts[0], @"^(\d+)([AB]?)$");
            var endMatch = Regex.Match(parts[1], @"^(\d+)([AB]?)$");

            if (!startMatch.Success || !endMatch.Success)
            {
                return [input];
            }

            int startNum = int.Parse(startMatch.Groups[1].Value);
            string sLetter = startMatch.Groups[2].Value;
            int endNum = int.Parse(endMatch.Groups[1].Value);
            string eLetter = endMatch.Groups[2].Value;

            bool startHasLetter = !string.IsNullOrEmpty(sLetter);
            bool endHasLetter = !string.IsNullOrEmpty(eLetter);

            if (startHasLetter != endHasLetter)
            {
                return [input];
            }

            int min = Math.Min(startNum, endNum);
            int max = Math.Max(startNum, endNum);
            List<int> numbers = [.. Enumerable.Range(min, max - min + 1)];

            List<string> result = [];

            if (startHasLetter)
            {
                if (sLetter == eLetter)
                {
                    numbers.ForEach(n => result.Add($"{n}{sLetter}"));
                }
                else
                {
                    numbers.ForEach(n =>
                    {
                        result.Add($"{n}A");
                        result.Add($"{n}B");
                    });
                }
            }
            else
            {
                numbers.ForEach(n => result.Add(n.ToString()));
            }

            return result;
        }
    }
}
